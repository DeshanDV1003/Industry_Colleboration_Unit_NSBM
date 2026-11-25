using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Venues;

/// <summary>A lecture hall, lab, auditorium or event hall the Assistant Registrar allocates.</summary>
public class Venue : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public VenueType Type { get; set; }
    public int Capacity { get; set; }
    public string? Facilities { get; set; }
    /// <summary>Inactive venues (under maintenance, closed) keep their history but take no new bookings.</summary>
    public bool IsActive { get; set; } = true;

    public string DisplayName => $"{Code} · {Name}";
}
