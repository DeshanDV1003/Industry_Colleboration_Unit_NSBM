using System.Text.RegularExpressions;

namespace NSBM.ICU.Application.Common.Reports;

/// <summary>Wording helpers shared by the reports.</summary>
public static partial class ReportText
{
    /// <summary>"InterviewInvited" → "Interview invited".</summary>
    public static string Humanize(Enum value)
    {
        var words = WordBoundary().Replace(value.ToString(), " $1").Trim();
        return words.Length == 0 ? words : words[..1] + words[1..].ToLowerInvariant();
    }

    public static string Range(DateOnly from, DateOnly to) => $"{from:d MMM yyyy} – {to:d MMM yyyy}";

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex WordBoundary();
}
