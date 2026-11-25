using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Rules;

/// <summary>Simple, transparent rules that turn answer counts into a practice signal (not a trained model).</summary>
public static class SkillSignalRules
{
    private const double StrongThreshold = 0.75;
    private const double DevelopingThreshold = 0.4;

    public static SkillSignal Classify(int correct, int total)
    {
        if (total <= 0) return SkillSignal.NeedsPractice;

        var ratio = (double)correct / total;
        return ratio >= StrongThreshold ? SkillSignal.Strong
             : ratio >= DevelopingThreshold ? SkillSignal.Developing
             : SkillSignal.NeedsPractice;
    }

    public static int Percent(int part, int whole) => whole <= 0 ? 0 : (int)Math.Round(100.0 * part / whole);
}
