namespace NSBM.ICU.Application.Common.Reports;

/// <summary>How a column's values are formatted in Excel and on screen.</summary>
public enum ReportColumnKind
{
    Text = 1,
    Number = 2,
    Date = 3,
    Time = 4,
    Percent = 5,
}

public sealed record ReportColumn(string Header, ReportColumnKind Kind = ReportColumnKind.Text);

/// <summary>
/// One report as a plain table, independent of output format: the web shows it and prints it, and
/// <see cref="Interfaces.IReportFileWriter"/> turns it into a spreadsheet. Each row has one value per column
/// (string, int, DateOnly, TimeOnly, a 0–100 percent as int, or null).
/// </summary>
public sealed record ReportTable(string Title, string Subtitle, IReadOnlyList<ReportColumn> Columns, IReadOnlyList<object?[]> Rows)
{
    /// <summary>Short machine name used for the file name, e.g. "hall-timetable".</summary>
    public string FileStem { get; init; } = "report";
}

/// <summary>A generated file ready to download.</summary>
public sealed record ReportFile(string FileName, string ContentType, byte[] Content);
