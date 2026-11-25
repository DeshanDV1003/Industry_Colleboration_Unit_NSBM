using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Application.Common.Extensions;

/// <summary>Resolves the role profile of the signed-in user, failing with 403 when the user has a different role.</summary>
public static class CurrentProfileExtensions
{
    public static async Task<Student> GetCurrentStudentAsync(this IApplicationDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        await db.Students.FirstOrDefaultAsync(s => s.UserId == currentUser.UserId, ct)
        ?? throw new ForbiddenAccessException("Only students can use this feature.");

    public static async Task<IndustryPartner> GetCurrentPartnerAsync(this IApplicationDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        await db.IndustryPartners.FirstOrDefaultAsync(p => p.UserId == currentUser.UserId, ct)
        ?? throw new ForbiddenAccessException("Only industry partners can use this feature.");

    public static async Task<Lecturer> GetCurrentLecturerAsync(this IApplicationDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        await db.Lecturers.FirstOrDefaultAsync(l => l.UserId == currentUser.UserId, ct)
        ?? throw new ForbiddenAccessException("Only lecturers can use this feature.");

    public static async Task<Administrator> GetCurrentAdministratorAsync(this IApplicationDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        await db.Administrators.FirstOrDefaultAsync(a => a.UserId == currentUser.UserId, ct)
        ?? throw new ForbiddenAccessException("Only Unit administrators can use this feature.");

    public static async Task<Registrar> GetCurrentRegistrarAsync(this IApplicationDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        await db.Registrars.FirstOrDefaultAsync(r => r.UserId == currentUser.UserId, ct)
        ?? throw new ForbiddenAccessException("Only the Assistant Registrar can use this feature.");

    /// <summary>Unit admins see every company's content; a partner sees only their own company's.</summary>
    public static async Task EnsureCompanyAccessAsync(this IApplicationDbContext db, ICurrentUser currentUser, int companyId, CancellationToken ct)
    {
        if (currentUser.Role == Roles.Admin) return;

        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        if (partner.CompanyId != companyId)
            throw new ForbiddenAccessException("This item belongs to another company.");
    }

    /// <summary>Returns the student's chosen career path id, or throws when no path has been chosen yet.</summary>
    public static int RequireCareerPathId(this Student student) =>
        student.CareerPathId ?? throw new BusinessRuleException("Choose a career path first.");
}
