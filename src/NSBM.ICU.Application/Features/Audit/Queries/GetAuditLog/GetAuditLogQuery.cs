using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Audit.Queries.GetAuditLog;

public sealed record AuditEntryDto(int Id, DateTime At, string ActorName, string? ActorRole, AuditCategory Category, string Action, string Summary);

public sealed record AuditPageDto(IReadOnlyList<AuditEntryDto> Items, int Total, int Page, int PageSize);

/// <summary>One page of the audit log, newest first.</summary>
public sealed record GetAuditLogQuery(AuditFilter Filter, int Page = 1) : IRequest<AuditPageDto>;

public sealed class GetAuditLogQueryValidator : AbstractValidator<GetAuditLogQuery>
{
    public GetAuditLogQueryValidator()
    {
        RuleFor(x => x.Filter).SetValidator(new AuditFilterValidator());
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    }
}

public sealed class GetAuditLogQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAuditLogQuery, AuditPageDto>
{
    public const int PageSize = 50;

    public async Task<AuditPageDto> Handle(GetAuditLogQuery request, CancellationToken ct)
    {
        var entries = db.Matching(request.Filter).AsNoTracking();
        var total = await entries.CountAsync(ct);
        var items = await entries
            .Skip((request.Page - 1) * PageSize).Take(PageSize)
            .Select(e => new AuditEntryDto(e.Id, e.At, e.ActorName, e.ActorRole, e.Category, e.Action, e.Summary))
            .ToListAsync(ct);
        return new AuditPageDto(items, total, request.Page, PageSize);
    }
}
