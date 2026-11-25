using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Domain.Entities.Learning;

/// <summary>One completed run of a career path's skills baseline. Students may retake it; the latest attempt counts.</summary>
public class BaselineAttempt : AuditableEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
    public DateTime CompletedAt { get; set; }
    public int CorrectCount { get; set; }
    public int QuestionCount { get; set; }

    public ICollection<BaselineAnswer> Answers { get; } = [];
}
