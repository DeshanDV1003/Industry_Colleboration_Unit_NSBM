using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Domain.Entities.Learning;

/// <summary>
/// A student's written answers to a task's follow-up questions — beginner-level practice evidence.
/// One submission per student and task; resubmitting replaces the answers.
/// </summary>
public class KnowledgeCheckSubmission : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int TaskId { get; set; }
    public IndustryTask? Task { get; set; }
    public DateTime SubmittedAt { get; set; }

    public ICollection<KnowledgeAnswer> Answers { get; } = [];
}
