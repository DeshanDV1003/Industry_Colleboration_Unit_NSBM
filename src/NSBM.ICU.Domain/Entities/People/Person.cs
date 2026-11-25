using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.People;

/// <summary>
/// Profile data shared by every role-specific profile. <see cref="UserId"/> links the profile to its
/// identity account, which lives in the infrastructure layer.
/// </summary>
public abstract class Person : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
