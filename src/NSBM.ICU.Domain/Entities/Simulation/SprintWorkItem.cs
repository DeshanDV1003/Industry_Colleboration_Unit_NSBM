using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Simulation;

/// <summary>
/// A work item on the virtual IT company's practice sprint board. Items belong to a role; items outside the
/// student's role are handled by fictional teammates so the board feels like real teamwork.
/// </summary>
public class SprintWorkItem : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StoryPoints { get; set; }
    public string TeammateName { get; set; } = string.Empty;
    /// <summary>Where the fictional teammate's copy of the item sits when the student does not own it.</summary>
    public SprintItemStatus TeammateStatus { get; set; }
    public int SortOrder { get; set; }
}
