using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Calendar.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar.Queries.GetCalendarOptions;

/// <summary>Career paths, approved tasks and approved vacancies the admin can attach to an event.</summary>
public sealed record GetCalendarOptionsQuery : IRequest<CalendarOptionsDto>;

public sealed class GetCalendarOptionsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetCalendarOptionsQuery, CalendarOptionsDto>
{
    public async Task<CalendarOptionsDto> Handle(GetCalendarOptionsQuery request, CancellationToken ct)
    {
        var paths = await db.CareerPaths.AsNoTracking().OrderBy(p => p.Name)
            .Select(p => new CalendarOptionDto(p.Id, p.Name)).ToListAsync(ct);
        var tasks = await db.IndustryTasks.AsNoTracking().Where(t => t.Status == ModerationStatus.Approved).OrderBy(t => t.Title)
            .Select(t => new CalendarOptionDto(t.Id, t.Title + " (" + t.Company!.Name + ")")).ToListAsync(ct);
        var vacancies = await db.Vacancies.AsNoTracking().Where(v => v.Status == ModerationStatus.Approved).OrderBy(v => v.Title)
            .Select(v => new CalendarOptionDto(v.Id, v.Title + " (" + v.Company!.Name + ")")).ToListAsync(ct);
        return new CalendarOptionsDto(paths, tasks, vacancies);
    }
}
