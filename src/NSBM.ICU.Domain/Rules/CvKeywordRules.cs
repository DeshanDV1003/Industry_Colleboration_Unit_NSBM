using System.Text.RegularExpressions;

namespace NSBM.ICU.Domain.Rules;

/// <summary>
/// Decides whether text mentions a catalogue skill. Skill names like "Git &amp; version control" or
/// "Data structures &amp; algorithms" are split into alternatives, so "git" or "algorithms" also count.
/// </summary>
public static class CvKeywordRules
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(200);

    public static bool Mentions(string text, string skillName) =>
        Aliases(skillName).Any(alias =>
            Regex.IsMatch(text, $@"(?<![\w#+]){Regex.Escape(alias)}(?![\w#+])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, Timeout));

    private static IEnumerable<string> Aliases(string skillName) =>
        skillName.Split(['&', '(', ')', '/'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Prepend(skillName)
            .Where(a => a.Length >= 2)
            .Distinct(StringComparer.OrdinalIgnoreCase);
}
