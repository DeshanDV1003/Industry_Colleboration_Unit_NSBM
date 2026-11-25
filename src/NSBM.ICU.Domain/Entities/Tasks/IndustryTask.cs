using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Tasks;

/// <summary>A company-style practice task authored by an industry partner.</summary>
public class IndustryTask : PartnerContent
{
    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public LearningStage Level { get; set; }
    public WorkSituation Situation { get; set; }
    public int EstimatedMinutes { get; set; }
    public string? StarterCode { get; set; }
    public string? StarterLanguage { get; set; }

    public ICollection<TaskSkill> Skills { get; } = [];
    public ICollection<TaskCheck> Checks { get; } = [];
    public ICollection<KnowledgeQuestion> KnowledgeQuestions { get; } = [];
}
