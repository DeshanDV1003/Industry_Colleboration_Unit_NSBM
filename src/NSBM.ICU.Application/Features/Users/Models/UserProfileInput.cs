using FluentValidation;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Application.Features.Users.Models;

/// <summary>
/// The editable details of any account. Only the fields of the account's role are used:
/// students (StudentNo, Programme, AcademicYear, Batch), lecturers (StaffNo, Designation, Department),
/// partners (CompanyId, JobTitle), admins (Designation) and the registrar (Designation, Office).
/// </summary>
public sealed record UserProfileInput(
    string FirstName,
    string LastName,
    string? StudentNo = null,
    string? Programme = null,
    int? AcademicYear = null,
    string? Batch = null,
    string? StaffNo = null,
    string? Designation = null,
    string? Department = null,
    int? CompanyId = null,
    string? JobTitle = null,
    string? Office = null);

/// <remarks>Internal: it needs the role, so it is built by the request validators, not registered for DI.</remarks>
internal sealed class UserProfileInputValidator : AbstractValidator<UserProfileInput>
{
    public const int MaxAcademicYear = 5;

    public UserProfileInputValidator(string role)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        switch (role)
        {
            case Roles.Student:
                RuleFor(x => x.StudentNo).NotEmpty().WithMessage("Enter the student number.").MaximumLength(30);
                RuleFor(x => x.Programme).NotEmpty().WithMessage("Enter the degree programme.").MaximumLength(150);
                RuleFor(x => x.AcademicYear).NotNull().WithMessage("Choose the academic year.").InclusiveBetween(1, MaxAcademicYear);
                RuleFor(x => x.Batch).NotEmpty().WithMessage("Enter the batch, e.g. 2024.1.").MaximumLength(20);
                break;
            case Roles.Lecturer:
                RuleFor(x => x.StaffNo).NotEmpty().WithMessage("Enter the staff number.").MaximumLength(30);
                RuleFor(x => x.Designation).NotEmpty().MaximumLength(150);
                RuleFor(x => x.Department).NotEmpty().MaximumLength(150);
                break;
            case Roles.Industry:
                RuleFor(x => x.CompanyId).NotNull().WithMessage("Choose the company.");
                RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(150);
                break;
            case Roles.Admin:
                RuleFor(x => x.Designation).NotEmpty().MaximumLength(150);
                break;
            case Roles.Registrar:
                RuleFor(x => x.Designation).NotEmpty().MaximumLength(150);
                RuleFor(x => x.Office).NotEmpty().MaximumLength(150);
                break;
        }
    }
}
