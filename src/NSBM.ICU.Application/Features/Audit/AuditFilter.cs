using FluentValidation;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Auditing;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Audit;

/// <summary>Filters shared by the audit log page and its export. Dates are inclusive days (UTC).</summary>
public sealed record AuditFilter(AuditCategory? Category, string? Search, DateOnly? From, DateOnly? To);

public sealed class AuditFilterValidator : AbstractValidator<AuditFilter>
{
    public AuditFilterValidator()
    {
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).When(x => x.From is not null && x.To is not null)
            .WithMessage("The end date cannot be before the start date.");
    }
}

public static class AuditFilterExtensions
{
    /// <summary>Entries matching the filter, newest first. Search looks at the actor, the summary, the action code and the subject (e.g. an account's e-mail, so one search shows everything done to that account).</summary>
    public static IQueryable<AuditEntry> Matching(this IApplicationDbContext db, AuditFilter filter)
    {
        var entries = db.AuditEntries.AsQueryable();
        if (filter.Category is { } category) entries = entries.Where(e => e.Category == category);
        if (filter.From is { } from) entries = entries.Where(e => e.At >= from.ToDateTime(TimeOnly.MinValue));
        if (filter.To is { } to) entries = entries.Where(e => e.At < to.AddDays(1).ToDateTime(TimeOnly.MinValue));
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            entries = entries.Where(e => e.ActorName.Contains(term) || e.Summary.Contains(term) || e.Action.Contains(term)
                                         || (e.SubjectId != null && e.SubjectId.Contains(term)));
        }
        return entries.OrderByDescending(e => e.At).ThenByDescending(e => e.Id);
    }
}
