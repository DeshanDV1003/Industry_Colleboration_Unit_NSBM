using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.UnitTests.Achievements;

public sealed class StreakRulesTests
{
    // Sunday 22 Nov 2026; its week starts on Monday 16 Nov.
    private static readonly DateOnly Today = new(2026, 11, 22);
    private static DateOnly WeeksAgo(int weeks, int day = 2) => new DateOnly(2026, 11, 16).AddDays(-7 * weeks + day);

    [Fact]
    public void Weeks_start_on_monday()
    {
        Assert.Equal(new DateOnly(2026, 11, 16), StreakRules.WeekStart(Today));
        Assert.Equal(new DateOnly(2026, 11, 16), StreakRules.WeekStart(new DateOnly(2026, 11, 16)));
    }

    [Fact]
    public void Streak_counts_consecutive_active_weeks_ending_this_week()
    {
        DateOnly[] days = [WeeksAgo(0), WeeksAgo(1), WeeksAgo(1, 4), WeeksAgo(2), WeeksAgo(4)];
        Assert.Equal(3, StreakRules.CurrentStreak(days, Today));
    }

    [Fact]
    public void Streak_stays_alive_until_the_week_ends()
    {
        DateOnly[] days = [WeeksAgo(1), WeeksAgo(2)]; // nothing yet this week
        Assert.Equal(2, StreakRules.CurrentStreak(days, Today));
        Assert.Equal(0, StreakRules.CurrentStreak([WeeksAgo(2)], Today)); // missed last week
        Assert.Equal(0, StreakRules.CurrentStreak([], Today));
    }

    [Fact]
    public void Best_streak_finds_the_longest_run()
    {
        DateOnly[] days = [WeeksAgo(10), WeeksAgo(9), WeeksAgo(8), WeeksAgo(8, 5), WeeksAgo(5), WeeksAgo(0)];
        Assert.Equal(3, StreakRules.BestStreak(days));
        Assert.Equal(0, StreakRules.BestStreak([]));
    }

    [Fact]
    public void Recent_weeks_are_oldest_first_ending_this_week()
    {
        var weeks = StreakRules.RecentWeeks([WeeksAgo(0), WeeksAgo(2)], Today, 4);
        Assert.Equal([false, true, false, true], weeks);
    }

    [Theory]
    [InlineData(0, 0, ProgressLevel.Explorer)]
    [InlineData(1, 6, ProgressLevel.Explorer)]
    [InlineData(2, 6, ProgressLevel.Builder)]
    [InlineData(3, 6, ProgressLevel.Practitioner)]
    [InlineData(5, 6, ProgressLevel.Advanced)]
    [InlineData(6, 6, ProgressLevel.RoadmapComplete)]
    public void Level_follows_roadmap_completion(int completed, int total, ProgressLevel expected) =>
        Assert.Equal(expected, ProgressLevelRules.LevelFor(completed, total));
}
