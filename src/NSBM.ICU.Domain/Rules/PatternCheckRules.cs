using System.Text.RegularExpressions;

namespace NSBM.ICU.Domain.Rules;

/// <summary>
/// Evaluates task pattern checks against submitted text. Patterns run with a strict timeout so a badly
/// written expression can never stall the server. Nothing is ever executed — this only reads text.
/// </summary>
public static class PatternCheckRules
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(200);
    private const RegexOptions Options = RegexOptions.Multiline | RegexOptions.CultureInvariant;

    public static bool IsValidPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) return false;
        try
        {
            _ = new Regex(pattern, Options, Timeout);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>True when the check passes: the pattern is found (or absent, when <paramref name="shouldMatch"/> is false).</summary>
    public static bool Passes(string code, string pattern, bool shouldMatch)
    {
        try
        {
            return Regex.IsMatch(code, pattern, Options, Timeout) == shouldMatch;
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}
