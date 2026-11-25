namespace NSBM.ICU.Domain.Entities.People;

/// <summary>University academic staff who guide and support students.</summary>
public class Lecturer : Person
{
    public string StaffNo { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
