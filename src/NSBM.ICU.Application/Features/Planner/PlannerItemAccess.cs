using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Planning;

namespace NSBM.ICU.Application.Features.Planner;

internal static class PlannerItemAccess
{
    /// <summary>Loads one of the signed-in student's own planner items (404 for anyone else's).</summary>
    public static async Task<PlannerItem> GetOwnPlannerItemAsync(this IApplicationDbContext db, ICurrentUser currentUser, int id, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        return await db.PlannerItems.FirstOrDefaultAsync(p => p.Id == id && p.StudentId == student.Id, ct)
               ?? throw NotFoundException.For("Planner item", id);
    }
}
