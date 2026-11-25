using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Support;

/// <summary>An academic feedback or support note a lecturer writes for a student. The student sees it in their portfolio.</summary>
public class SupportNote : BaseEntity
{
    public int StudentId { get; set; }
    public int LecturerId { get; set; }
    public string LecturerName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
