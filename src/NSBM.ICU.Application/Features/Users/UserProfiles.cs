using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Users.Models;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Application.Features.Users;

/// <summary>Maps between the role profile entities and <see cref="UserProfileInput"/>, and enforces their unique numbers.</summary>
public static class UserProfiles
{
    public static Person New(string role) => role switch
    {
        Roles.Student => new Student(),
        Roles.Lecturer => new Lecturer(),
        Roles.Industry => new IndustryPartner(),
        Roles.Admin => new Administrator(),
        Roles.Registrar => new Registrar(),
        _ => throw new BusinessRuleException($"Unknown role '{role}'."),
    };

    /// <summary>Copies the input onto the profile; only the fields of the profile's role are read.</summary>
    public static void Apply(Person profile, UserProfileInput input)
    {
        profile.FirstName = input.FirstName.Trim();
        profile.LastName = input.LastName.Trim();

        switch (profile)
        {
            case Student s:
                s.StudentNo = input.StudentNo!.Trim().ToUpperInvariant();
                s.Programme = input.Programme!.Trim();
                s.AcademicYear = input.AcademicYear!.Value;
                s.Batch = input.Batch!.Trim();
                break;
            case Lecturer l:
                l.StaffNo = input.StaffNo!.Trim().ToUpperInvariant();
                l.Designation = input.Designation!.Trim();
                l.Department = input.Department!.Trim();
                break;
            case IndustryPartner p:
                p.CompanyId = input.CompanyId!.Value;
                p.JobTitle = input.JobTitle!.Trim();
                break;
            case Administrator a:
                a.Designation = input.Designation!.Trim();
                break;
            case Registrar r:
                r.Designation = input.Designation!.Trim();
                r.Office = input.Office!.Trim();
                break;
        }
    }

    public static UserProfileInput ToInput(Person profile) => profile switch
    {
        Student s => new(s.FirstName, s.LastName, StudentNo: s.StudentNo, Programme: s.Programme, AcademicYear: s.AcademicYear, Batch: s.Batch),
        Lecturer l => new(l.FirstName, l.LastName, StaffNo: l.StaffNo, Designation: l.Designation, Department: l.Department),
        IndustryPartner p => new(p.FirstName, p.LastName, CompanyId: p.CompanyId, JobTitle: p.JobTitle),
        Administrator a => new(a.FirstName, a.LastName, Designation: a.Designation),
        Registrar r => new(r.FirstName, r.LastName, Designation: r.Designation, Office: r.Office),
        _ => new(profile.FirstName, profile.LastName),
    };

    /// <summary>The profile of any role for an account, tracked so it can be edited.</summary>
    public static async Task<Person> RequireAsync(this IApplicationDbContext db, string userId, CancellationToken ct) =>
        (Person?)await db.Students.FirstOrDefaultAsync(x => x.UserId == userId, ct)
        ?? (Person?)await db.Lecturers.FirstOrDefaultAsync(x => x.UserId == userId, ct)
        ?? (Person?)await db.IndustryPartners.FirstOrDefaultAsync(x => x.UserId == userId, ct)
        ?? (Person?)await db.Administrators.FirstOrDefaultAsync(x => x.UserId == userId, ct)
        ?? await db.Registrars.FirstOrDefaultAsync(x => x.UserId == userId, ct)
        ?? throw NotFoundException.For("User", userId);

    /// <summary>Student and staff numbers identify people outside the system, so they must stay unique; the company must exist.</summary>
    public static async Task EnsureValidAsync(this IApplicationDbContext db, Person profile, CancellationToken ct)
    {
        switch (profile)
        {
            case Student s when await db.Students.AnyAsync(x => x.StudentNo == s.StudentNo && x.Id != s.Id, ct):
                throw new BusinessRuleException($"Student number {s.StudentNo} is already in use.");
            case Lecturer l when await db.Lecturers.AnyAsync(x => x.StaffNo == l.StaffNo && x.Id != l.Id, ct):
                throw new BusinessRuleException($"Staff number {l.StaffNo} is already in use.");
            case IndustryPartner p when !await db.Companies.AnyAsync(c => c.Id == p.CompanyId, ct):
                throw NotFoundException.For("Company", p.CompanyId);
        }
    }
}
