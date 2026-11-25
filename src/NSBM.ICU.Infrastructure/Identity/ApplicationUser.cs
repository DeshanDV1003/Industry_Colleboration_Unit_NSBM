using Microsoft.AspNetCore.Identity;

namespace NSBM.ICU.Infrastructure.Identity;

/// <summary>Identity account. Role-specific profile data lives in the domain <c>People</c> entities.</summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
