using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Events;

/// <summary>A student team for a team event. Members are the registrations that point at it.</summary>
public class EventTeam : BaseEntity
{
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty;
    public int CaptainStudentId { get; set; }
    public DateTime CreatedAt { get; set; }
}
