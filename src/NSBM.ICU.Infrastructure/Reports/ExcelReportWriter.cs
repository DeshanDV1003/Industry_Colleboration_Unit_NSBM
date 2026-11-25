using ClosedXML.Excel;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;

namespace NSBM.ICU.Infrastructure.Reports;

/// <summary>
/// Writes a report as an .xlsx workbook: title and subtitle, a frozen, filterable header row in NSBM green,
/// and typed cells (real dates, times and numbers, so Excel can sort and sum them).
/// </summary>
public sealed class ExcelReportWriter : IReportFileWriter
{
    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private static readonly XLColor HeaderFill = XLColor.FromHtml("#0B7A43");
    private const int HeaderRow = 4;

    public ReportFile WriteExcel(ReportTable table, DateTime generatedAt)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(SheetName(table.Title));

        sheet.Cell(1, 1).Value = table.Title;
        sheet.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);
        sheet.Cell(2, 1).Value = $"{table.Subtitle} · Generated {generatedAt:d MMM yyyy HH:mm} UTC · NSBM Industry Collaboration Unit";
        sheet.Cell(2, 1).Style.Font.SetFontColor(XLColor.Gray);

        for (var c = 0; c < table.Columns.Count; c++)
        {
            var cell = sheet.Cell(HeaderRow, c + 1);
            cell.Value = table.Columns[c].Header;
            cell.Style.Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(HeaderFill);
        }

        for (var r = 0; r < table.Rows.Count; r++)
            for (var c = 0; c < table.Columns.Count; c++)
                Write(sheet.Cell(HeaderRow + 1 + r, c + 1), table.Rows[r][c], table.Columns[c].Kind);

        if (table.Rows.Count > 0)
            sheet.Range(HeaderRow, 1, HeaderRow + table.Rows.Count, table.Columns.Count).SetAutoFilter();
        sheet.SheetView.FreezeRows(HeaderRow);
        sheet.Columns(1, table.Columns.Count).AdjustToContents(HeaderRow, HeaderRow + Math.Max(table.Rows.Count, 1), 8, 60);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new ReportFile($"{table.FileStem}-{generatedAt:yyyy-MM-dd}.xlsx", ContentType, stream.ToArray());
    }

    private static void Write(IXLCell cell, object? value, ReportColumnKind kind)
    {
        switch (value)
        {
            case null:
                return;
            case DateOnly date:
                cell.Value = date.ToDateTime(TimeOnly.MinValue);
                cell.Style.DateFormat.Format = "d mmm yyyy";
                return;
            case TimeOnly time:
                cell.Value = time.ToTimeSpan();
                cell.Style.DateFormat.Format = "hh:mm";
                return;
            case int number when kind == ReportColumnKind.Percent:
                cell.Value = number / 100.0;
                cell.Style.NumberFormat.Format = "0%";
                return;
            case int number:
                cell.Value = number;
                return;
            case bool flag:
                cell.Value = flag ? "Yes" : "No";
                return;
            default:
                cell.Value = value.ToString();
                return;
        }
    }

    /// <summary>Excel sheet names are at most 31 characters and cannot contain : \ / ? * [ ].</summary>
    private static string SheetName(string title)
    {
        var clean = new string(title.Where(ch => ch is not (':' or '\\' or '/' or '?' or '*' or '[' or ']')).ToArray());
        return clean.Length <= 31 ? clean : clean[..31];
    }
}
