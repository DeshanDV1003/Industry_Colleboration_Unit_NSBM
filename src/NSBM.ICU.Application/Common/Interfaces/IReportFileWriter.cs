using NSBM.ICU.Application.Common.Reports;

namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>Turns a report table into a downloadable spreadsheet.</summary>
public interface IReportFileWriter
{
    ReportFile WriteExcel(ReportTable table, DateTime generatedAt);
}
