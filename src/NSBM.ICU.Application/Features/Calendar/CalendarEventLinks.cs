using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Calendar.Models;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar;

/// <summary>Shared by create and update: checks linked records exist, then copies the input onto the event.</summary>
internal static class CalendarEventLinks
{
    public static async Task EnsureLinksAsync(this IApplicationDbContext db, CalendarEventInput input, CancellationToken ct)
    {
        if (input.CareerPathId is { } pathId && !await db.CareerPaths.AnyAsync(p => p.Id == pathId, ct))
            throw NotFoundException.For("Career path", pathId);
        if (input.IndustryTaskId is { } taskId && !await db.IndustryTasks.AnyAsync(t => t.Id == taskId && t.Status == ModerationStatus.Approved, ct))
            throw new BusinessRuleException("Only approved industry tasks can be released on the calendar.");
        if (input.VacancyId is { } vacancyId && !await db.Vacancies.AnyAsync(v => v.Id == vacancyId && v.Status == ModerationStatus.Approved, ct))
            throw new BusinessRuleException("Only approved vacancies can be linked to the calendar.");
    }

    public static void Apply(this CalendarEvent target, CalendarEventInput input)
    {
        target.Title = input.Title.Trim();
        target.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
        target.Type = input.Type;
        target.Audience = input.Audience;
        target.StartDate = input.StartDate;
        target.EndDate = input.EndDate;
        target.Location = string.IsNullOrWhiteSpace(input.Location) ? null : input.Location.Trim();
        target.CareerPathId = input.CareerPathId;
        target.IndustryTaskId = input.IndustryTaskId;
        target.VacancyId = input.VacancyId;
    }
}
