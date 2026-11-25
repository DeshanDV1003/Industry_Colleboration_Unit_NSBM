namespace NSBM.ICU.Domain.Entities.People;

/// <summary>The Assistant Registrar, who allocates lecture halls to student cohorts and halls to events.</summary>
public class Registrar : Person
{
    public string Designation { get; set; } = string.Empty;
    public string Office { get; set; } = string.Empty;
}
