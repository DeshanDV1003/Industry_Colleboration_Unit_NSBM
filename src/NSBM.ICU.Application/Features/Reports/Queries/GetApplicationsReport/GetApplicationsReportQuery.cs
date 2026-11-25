using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Reports.Queries.GetApplicationsReport;

/// <summary>Internship applications submitted in a date range, optionally one status, with where each one stands.</summary>
public sealed record GetApplicationsReportQuery(DateOnly From, DateOnly To, ApplicationStatus? Status) : IRequest<ReportTable>;

public sealed class GetApplicationsReportQueryValidator : AbstractValidator<GetApplicationsReportQuery>
{
    public GetApplicationsReportQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).WithMessage("The end date cannot be before the start date.");
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class GetApplicationsReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetApplicationsReportQuery, ReportTable>
{
    private static readonly ReportColumn[] Columns =
    [
        new("Submitted", ReportColumnKind.Date), new("Company"), new("Vacancy"), new("Career path"), new("Student no."), new("Student"),
        new("Programme"), new("Skill overlap", ReportColumnKind.Percent), new("Status"), new("Status changed", ReportColumnKind.Date),
    ];

    public async Task<ReportTable> Handle(GetApplicationsReportQuery request, CancellationToken ct)
    {
        var start = request.From.ToDateTime(TimeOnly.MinValue);
        var end = request.To.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var applications = await db.InternshipApplications.AsNoTracking()
            .Where(a => a.SubmittedAt >= start && a.SubmittedAt < end)
            .Where(a => request.Status == null || a.Status == request.Status)
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => new
            {
                a.SubmittedAt, Company = a.Vacancy!.Company!.Name, Vacancy = a.Vacancy.Title, Path = a.Vacancy.CareerPath!.Name,
                a.Student!.StudentNo, Student = a.Student.FirstName + " " + a.Student.LastName, a.Student.Programme,
                a.MatchPercentAtSubmission, a.Status, a.StatusChangedAt,
            })
            .ToListAsync(ct);

        var rows = applications.Select(a => new object?[]
        {
            DateOnly.FromDateTime(a.SubmittedAt), a.Company, a.Vacancy, a.Path, a.StudentNo, a.Student, a.Programme,
            a.MatchPercentAtSubmission, ReportText.Humanize(a.Status), DateOnly.FromDateTime(a.StatusChangedAt),
        }).ToList();

        var scope = ReportText.Range(request.From, request.To) + (request.Status is { } s ? $" · {ReportText.Humanize(s)} only" : string.Empty);
        return new ReportTable("Internship applications", $"{scope} · {rows.Count} applications · skill overlap is a practice signal, not a hiring prediction",
            Columns, rows) { FileStem = "internship-applications" };
    }
}
