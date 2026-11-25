namespace NSBM.ICU.Domain.Entities.People;

/// <summary>Industry Collaboration Unit staff who moderate content and manage the platform.</summary>
public class Administrator : Person
{
    public string Designation { get; set; } = string.Empty;
}
