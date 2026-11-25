using System.Text.RegularExpressions;

namespace NSBM.ICU.Domain.Rules;

/// <summary>
/// Simple keyword and length checks for practice answers. This is not AI grading and never blocks a student —
/// it only points at topics a fuller answer could cover.
/// </summary>
public static partial class InterviewFeedbackRules
{
    public const int ShortAnswerWords = 40;
    public const int LongAnswerWords = 250;

    public static int CountWords(string text) => WordPattern().Matches(text).Count;

    /// <summary>Keywords that appear in the answer (case-insensitive, whole words or phrases).</summary>
    public static IReadOnlyList<string> MatchedKeywords(string text, IEnumerable<string> keywords) =>
        keywords.Where(k => Regex.IsMatch(text, $@"\b{Regex.Escape(k)}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).ToList();

    /// <summary>Short, kind notes based on length and topic coverage.</summary>
    public static IReadOnlyList<string> Notes(int wordCount, int matched, int total)
    {
        var notes = new List<string>();
        if (wordCount < ShortAnswerWords)
            notes.Add("Your answer is quite short. Add a real example: the situation, what you did and the result.");
        else if (wordCount > LongAnswerWords)
            notes.Add("Your answer is long. In an interview, aim for one or two minutes — keep the strongest example.");

        var coverage = SkillSignalRules.Percent(matched, total);
        notes.Add(coverage >= 60
            ? "You covered most of the key topics for this question. Well done."
            : "Try to mention more of the key topics listed below — they are what interviewers usually listen for.");
        return notes;
    }

    [GeneratedRegex(@"\b[\w'-]+\b")]
    private static partial Regex WordPattern();
}
