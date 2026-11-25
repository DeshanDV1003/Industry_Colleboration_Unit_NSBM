using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Common.Validation;
using NSBM.ICU.Application.Features.Users.Dtos;
using NSBM.ICU.Application.Features.Users.Models;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Users.Commands.ImportStudents;

/// <summary>
/// Opens accounts for a cohort from CSV text, all with the same temporary password. Valid rows are created;
/// every other row is reported with its reason, so the admin can fix and re-import just those.
/// </summary>
public sealed record ImportStudentsCommand(string Csv, string TemporaryPassword) : IRequest<StudentImportResultDto>;

public sealed class ImportStudentsCommandValidator : AbstractValidator<ImportStudentsCommand>
{
    public const int MaxLength = 200_000;

    public ImportStudentsCommandValidator()
    {
        RuleFor(x => x.Csv).NotEmpty().WithMessage("Paste or choose a CSV file.").MaximumLength(MaxLength);
        RuleFor(x => x.TemporaryPassword).ValidPassword();
    }
}

/// <summary>The same rules a single student account gets, applied to one CSV row.</summary>
public sealed class StudentCsvRowValidator : AbstractValidator<StudentCsvRow>
{
    public StudentCsvRowValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.AcademicYear)
            .Must(y => int.TryParse(y, out var n) && n is >= 1 and <= UserProfileInputValidator.MaxAcademicYear)
            .WithMessage($"AcademicYear must be a number from 1 to {UserProfileInputValidator.MaxAcademicYear}.");
        RuleFor(x => ImportStudentsCommandHandler.ToInput(x)).SetValidator(new UserProfileInputValidator(Roles.Student))
            .When(x => int.TryParse(x.AcademicYear, out _)).OverridePropertyName("Student");
    }
}

public sealed class ImportStudentsCommandHandler(IApplicationDbContext db, IIdentityService identity, IAuditTrail audit)
    : IRequestHandler<ImportStudentsCommand, StudentImportResultDto>
{
    public const int MaxRows = 500;
    private static readonly StudentCsvRowValidator RowValidator = new();

    public async Task<StudentImportResultDto> Handle(ImportStudentsCommand request, CancellationToken ct)
    {
        var parsed = StudentCsv.Parse(request.Csv);
        if (parsed.Error is not null) throw new BusinessRuleException(parsed.Error);
        if (parsed.Rows.Count == 0) throw new BusinessRuleException("The file has a header but no students.");
        if (parsed.Rows.Count > MaxRows) throw new BusinessRuleException($"Import at most {MaxRows} students at a time.");

        var errors = new List<StudentImportRowError>();
        var created = 0;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in parsed.Rows)
        {
            var problem = await TryCreateAsync(row, request.TemporaryPassword, seen, ct);
            if (problem is null) created++;
            else errors.Add(new StudentImportRowError(row.Row, string.IsNullOrWhiteSpace(row.StudentNo) ? null : row.StudentNo, problem));
        }

        await audit.RecordAsync(AuditCategory.Accounts, AuditActions.StudentsImported,
            $"Imported {created} student account{(created == 1 ? "" : "s")} from CSV ({errors.Count} row{(errors.Count == 1 ? "" : "s")} not imported)",
            cancellationToken: ct);
        await db.SaveChangesAsync(ct);
        return new StudentImportResultDto(created, errors);
    }

    public static UserProfileInput ToInput(StudentCsvRow row) => new(row.FirstName, row.LastName,
        StudentNo: row.StudentNo, Programme: row.Programme, AcademicYear: int.TryParse(row.AcademicYear, out var y) ? y : null, Batch: row.Batch);

    /// <summary>Creates one student; returns why it could not be created, or null on success.</summary>
    private async Task<string?> TryCreateAsync(StudentCsvRow row, string password, HashSet<string> seen, CancellationToken ct)
    {
        var invalid = (await RowValidator.ValidateAsync(row, ct)).Errors.FirstOrDefault();
        if (invalid is not null) return invalid.ErrorMessage;
        if (!seen.Add("no:" + row.StudentNo) || !seen.Add("mail:" + row.Email))
            return "This student appears more than once in the file.";

        var profile = UserProfiles.New(Roles.Student);
        UserProfiles.Apply(profile, ToInput(row));
        try
        {
            await db.EnsureValidAsync(profile, ct);
            var account = new NewAccount(Roles.Student, row.Email.Trim().ToLowerInvariant(), profile.FirstName, profile.LastName, password);
            await identity.CreateAccountAsync(account, profile, ct);
            return null;
        }
        catch (BusinessRuleException e) { return e.Message; }
        catch (Common.Exceptions.ValidationException e) { return string.Join(" ", e.Errors.SelectMany(x => x.Value)); }
    }
}
