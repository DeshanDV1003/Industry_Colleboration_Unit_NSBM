using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Rules;

/// <summary>Maps roadmap completion to a progress level shown on the student's achievements and profile card.</summary>
public static class ProgressLevelRules
{
    public static ProgressLevel LevelFor(int completedSteps, int totalSteps)
    {
        if (totalSteps <= 0) return ProgressLevel.Explorer;
        var percent = completedSteps * 100 / totalSteps;
        return percent switch
        {
            >= 100 => ProgressLevel.RoadmapComplete,
            >= 75 => ProgressLevel.Advanced,
            >= 50 => ProgressLevel.Practitioner,
            >= 25 => ProgressLevel.Builder,
            _ => ProgressLevel.Explorer,
        };
    }
}
