using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Tasks;

/// <summary>
/// A simple pattern check run against a submitted attempt. It looks at the text of the code only —
/// the code is never executed.
/// </summary>
public class TaskCheck : BaseEntity
{
    public int TaskId { get; set; }
    public int SortOrder { get; set; }
    public string Description { get; set; } = string.Empty;
    /// <summary>A .NET regular expression matched against the submission.</summary>
    public string Pattern { get; set; } = string.Empty;
    /// <summary>True when the pattern must appear; false when it must not appear.</summary>
    public bool ShouldMatch { get; set; } = true;
    public string? Hint { get; set; }
}
