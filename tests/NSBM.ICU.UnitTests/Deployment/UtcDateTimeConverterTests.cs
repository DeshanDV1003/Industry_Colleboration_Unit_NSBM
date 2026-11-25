using System.Text.Json;
using NSBM.ICU.API.Serialization;

namespace NSBM.ICU.UnitTests.Deployment;

public sealed class UtcDateTimeConverterTests
{
    private static readonly JsonSerializerOptions Options = new() { Converters = { new UtcDateTimeConverter() } };

    [Fact]
    public void Times_from_the_database_are_written_as_utc() =>
        Assert.Equal("\"2026-11-25T09:30:00Z\"", JsonSerializer.Serialize(new DateTime(2026, 11, 25, 9, 30, 0, DateTimeKind.Unspecified), Options));

    [Fact]
    public void Fractions_of_a_second_are_kept() =>
        Assert.Equal("\"2026-11-25T09:30:00.25Z\"", JsonSerializer.Serialize(new DateTime(2026, 11, 25, 9, 30, 0, 250, DateTimeKind.Utc), Options));

    [Fact]
    public void Times_sent_with_an_offset_are_read_as_utc()
    {
        var value = JsonSerializer.Deserialize<DateTime>("\"2026-11-25T15:00:00+05:30\"", Options);
        Assert.Equal(DateTimeKind.Utc, value.Kind);
        Assert.Equal(new DateTime(2026, 11, 25, 9, 30, 0), value);
    }

    [Fact]
    public void Times_sent_without_a_zone_are_taken_as_utc() =>
        Assert.Equal(DateTimeKind.Utc, JsonSerializer.Deserialize<DateTime>("\"2026-11-25T09:30:00\"", Options).Kind);
}
