using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;

namespace NSBM.ICU.Application.Features.Audit.Queries.GetAuditReport;

/// <summary>The filtered audit log as a report table, for Excel. Capped so an export stays a sensible size.</summary>
public sealed record GetAuditReportQuery(AuditFilter Filter) : IRequest<ReportTable>;

public sealed class GetAuditReportQueryValidator : AbstractValidator<GetAuditReportQuery>
{
    public GetAuditReportQueryValidator() => RuleFor(x => x.Filter).SetValidator(new AuditFilterValidator());
}

public sealed class GetAuditReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAuditReportQuery, ReportTable>
{
    public const int MaxRows = 10_000;

    private static readonly ReportColumn[] Columns =
    [
        new("Date (UTC)", ReportColumnKind.Date), new("Time (UTC)", ReportColumnKind.Time), new("Who"), new("Role"), new("Area"), new("Action"), new("What happened"),
    ];

    public async Task<ReportTable> Handle(GetAuditReportQuery request, CancellationToken ct)
    {
        var entries = await db.Matching(request.Filter).AsNoTracking().Take(MaxRows + 1)
            .Select(e => new { e.At, e.ActorName, e.ActorRole, e.Category, e.Action, e.Summary })
            .ToListAsync(ct);

        var capped = entries.Count > MaxRows;
        var rows = entries.Take(MaxRows).Select(e => new object?[]
        {
            DateOnly.FromDateTime(e.At), TimeOnly.FromDateTime(e.At), e.ActorName, e.ActorRole, e.Category.ToString(), e.Action, e.Summary,
        }).ToList();

        var scope = new List<string>();
        if (request.Filter.Category is { } c) scope.Add(c.ToString());
        if (request.Filter.From is { } from) scope.Add($"from {from:d MMM yyyy}");
        if (request.Filter.To is { } to) scope.Add($"to {to:d MMM yyyy}");
        if (!string.IsNullOrWhiteSpace(request.Filter.Search)) scope.Add($"matching \"{request.Filter.Search.Trim()}\"");
        var subtitle = (scope.Count == 0 ? "All entries" : string.Join(" · ", scope))
                       + $" · {rows.Count} entries" + (capped ? $" (first {MaxRows}; narrow the filters for the rest)" : string.Empty);

        return new ReportTable("Audit log", subtitle, Columns, rows) { FileStem = "audit-log" };
    }
}
