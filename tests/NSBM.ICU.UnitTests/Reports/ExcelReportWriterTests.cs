using ClosedXML.Excel;
using NSBM.ICU.Application.Common.Reports;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Reports;

namespace NSBM.ICU.UnitTests.Reports;

public sealed class ExcelReportWriterTests
{
    private static readonly DateTime GeneratedAt = new(2026, 11, 25, 9, 30, 0);

    private static readonly ReportTable Table = new("Hall timetable", "25 Nov 2026 – 22 Dec 2026",
        [new("Date", ReportColumnKind.Date), new("From", ReportColumnKind.Time), new("Venue"), new("Attendees", ReportColumnKind.Number),
         new("Overlap", ReportColumnKind.Percent), new("Attended")],
        [[new DateOnly(2026, 11, 30), new TimeOnly(9, 0), "LH-101 · Lecture Hall 101", 2, 75, true],
         [new DateOnly(2026, 12, 1), new TimeOnly(13, 0), "LAB-01 · Computer Lab 01", 40, null, false]])
    {
        FileStem = "hall-timetable",
    };

    private static IXLWorksheet Sheet(ReportFile file) => new XLWorkbook(new MemoryStream(file.Content)).Worksheet(1);

    [Fact]
    public void Names_the_file_after_the_report_and_day()
    {
        var file = new ExcelReportWriter().WriteExcel(Table, GeneratedAt);
        Assert.Equal("hall-timetable-2026-11-25.xlsx", file.FileName);
        Assert.StartsWith("application/vnd.openxmlformats", file.ContentType);
    }

    [Fact]
    public void Writes_title_header_and_typed_cells()
    {
        var sheet = Sheet(new ExcelReportWriter().WriteExcel(Table, GeneratedAt));

        Assert.Equal("Hall timetable", sheet.Cell(1, 1).GetString());
        Assert.Contains("25 Nov 2026 – 22 Dec 2026", sheet.Cell(2, 1).GetString());
        Assert.Equal("Venue", sheet.Cell(4, 3).GetString());

        Assert.Equal(new DateTime(2026, 11, 30), sheet.Cell(5, 1).GetDateTime());
        Assert.Equal(TimeSpan.FromHours(9), sheet.Cell(5, 2).GetTimeSpan());
        Assert.Equal(2, sheet.Cell(5, 4).GetValue<int>());
        Assert.Equal(0.75, sheet.Cell(5, 5).GetDouble(), 3);
        Assert.Equal("Yes", sheet.Cell(5, 6).GetString());
        Assert.True(sheet.Cell(6, 5).IsEmpty());
    }

    [Fact]
    public void Header_stays_visible_and_filterable()
    {
        var sheet = Sheet(new ExcelReportWriter().WriteExcel(Table, GeneratedAt));
        Assert.Equal(4, sheet.SheetView.SplitRow);
        Assert.True(sheet.AutoFilter.IsEnabled);
    }

    [Fact]
    public void An_empty_report_still_opens() =>
        Assert.Equal("Venue", Sheet(new ExcelReportWriter().WriteExcel(Table with { Rows = [] }, GeneratedAt)).Cell(4, 3).GetString());

    [Theory]
    [InlineData(ApplicationStatus.InterviewInvited, "Interview invited")]
    [InlineData(ApplicationStatus.Submitted, "Submitted")]
    [InlineData(VenueBookingPurpose.IndustryEvent, "Industry event")]
    public void Enum_values_read_as_words(Enum value, string expected) => Assert.Equal(expected, ReportText.Humanize(value));
}
