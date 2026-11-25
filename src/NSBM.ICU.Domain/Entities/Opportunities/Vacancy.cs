using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Opportunities;

/// <summary>A fictional internship vacancy posted by a partner company. Students can search it after approval.</summary>
public class Vacancy : PartnerContent
{
    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkMode WorkMode { get; set; }
    public string Location { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public DateOnly ClosingDate { get; set; }

    public ICollection<VacancySkill> Skills { get; } = [];

    /// <summary>Open vacancies accept applications up to and including the closing date.</summary>
    public bool IsOpenOn(DateOnly date) => Status == ModerationStatus.Approved && date <= ClosingDate;
}
