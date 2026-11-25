using NSBM.ICU.Domain.Entities.Organisations;

namespace NSBM.ICU.Domain.Entities.People;

/// <summary>A representative of a partner company who publishes tasks and vacancies and mentors students.</summary>
public class IndustryPartner : Person
{
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public string JobTitle { get; set; } = string.Empty;
}
