namespace NSBM.ICU.Domain.Rules;

/// <summary>
/// Weekly practice streaks. A week (Monday to Sunday) counts when the student practised at least once. Weekly, not
/// daily, so streaks reward steady habits without pressuring students to log in every day.
/// </summary>
public static class StreakRules
{
    public static DateOnly WeekStart(DateOnly day) => day.AddDays(-(((int)day.DayOfWeek + 6) % 7));

    /// <summary>
    /// Consecutive active weeks ending this week. If nothing has happened yet this week, the streak counts up to
    /// last week, because the student still has until Sunday to keep it going.
    /// </summary>
    public static int CurrentStreak(IEnumerable<DateOnly> activityDays, DateOnly today)
    {
        var weeks = activityDays.Select(WeekStart).ToHashSet();
        var week = WeekStart(today);
        if (!weeks.Contains(week)) week = week.AddDays(-7);

        var streak = 0;
        while (weeks.Contains(week))
        {
            streak++;
            week = week.AddDays(-7);
        }
        return streak;
    }

    public static int BestStreak(IEnumerable<DateOnly> activityDays)
    {
        var weeks = activityDays.Select(WeekStart).Distinct().Order().ToList();
        int best = 0, run = 0;
        for (var i = 0; i < weeks.Count; i++)
        {
            run = i > 0 && weeks[i].DayNumber - weeks[i - 1].DayNumber == 7 ? run + 1 : 1;
            best = Math.Max(best, run);
        }
        return best;
    }

    /// <summary>Whether each of the last <paramref name="count"/> weeks was active, oldest first, ending this week.</summary>
    public static IReadOnlyList<bool> RecentWeeks(IEnumerable<DateOnly> activityDays, DateOnly today, int count)
    {
        var weeks = activityDays.Select(WeekStart).ToHashSet();
        var thisWeek = WeekStart(today);
        return Enumerable.Range(0, count).Select(i => weeks.Contains(thisWeek.AddDays(-7 * (count - 1 - i)))).ToList();
    }
}
