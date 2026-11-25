using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Domain.Entities.Mentoring;

/// <summary>A student's question to industry mentors. Any partner can reply; the first reply answers it.</summary>
public class MentorQuestion : AuditableEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int? CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime AskedAt { get; set; }

    public string? Reply { get; private set; }
    public string? RepliedBy { get; private set; }
    public string? RepliedByCompany { get; private set; }
    public DateTime? RepliedAt { get; private set; }

    public bool IsAnswered => Reply is not null;

    public void Answer(string reply, string mentorName, string companyName, DateTime at)
    {
        Reply = reply;
        RepliedBy = mentorName;
        RepliedByCompany = companyName;
        RepliedAt = at;
    }
}
