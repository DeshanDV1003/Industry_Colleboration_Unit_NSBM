using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Domain.Entities.Support;

/// <summary>A lecturer suggests a specific practice task to a student. It shows up in the student's task library.</summary>
public class TaskAssignment : BaseEntity
{
    public int StudentId { get; set; }
    public int TaskId { get; set; }
    public IndustryTask? Task { get; set; }
    public int LecturerId { get; set; }
    public string LecturerName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime AssignedAt { get; set; }
}
