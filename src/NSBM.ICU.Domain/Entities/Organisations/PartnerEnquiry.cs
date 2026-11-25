using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Organisations;

/// <summary>A message from a company that wants to partner with the Unit, sent from the public website.</summary>
public class PartnerEnquiry : BaseEntity
{
    public string OrganisationName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public PartnerInterest Interest { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }

    public bool IsHandled { get; private set; }
    public DateTime? HandledAt { get; private set; }
    public string? HandledBy { get; private set; }

    public void MarkHandled(string by, DateTime at)
    {
        if (IsHandled) return;
        IsHandled = true;
        HandledBy = by;
        HandledAt = at;
    }
}
