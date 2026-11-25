using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.UnitTests.Analytics;

public sealed class PrivacyRulesTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(40, true)]
    public void Rates_need_at_least_three_people(int groupSize, bool shown) =>
        Assert.Equal(shown, PrivacyRules.CanShowRate(groupSize));
}
