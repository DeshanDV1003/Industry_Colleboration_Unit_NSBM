using System.Text;

namespace NSBM.ICU.Application.Features.Users;

/// <summary>One student row from an import file, before validation.</summary>
public sealed record StudentCsvRow(int Row, string StudentNo, string FirstName, string LastName, string Email, string Programme, string AcademicYear, string Batch);

/// <summary>
/// Reads the student import format: a header row with the columns below (any order, case ignored), then one
/// student per line. Commas inside a value need double quotes, as spreadsheet programs write them. Pure, so it is unit tested.
/// </summary>
public static class StudentCsv
{
    public static readonly string[] Columns = ["StudentNo", "FirstName", "LastName", "Email", "Programme", "AcademicYear", "Batch"];

    public sealed record ParseResult(IReadOnlyList<StudentCsvRow> Rows, string? Error);

    public static ParseResult Parse(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n')
            .Select((line, i) => (Row: i + 1, Line: line))
            .Where(l => !string.IsNullOrWhiteSpace(l.Line))
            .ToList();
        if (lines.Count == 0)
            return new([], "The file is empty.");

        var header = SplitLine(lines[0].Line).Select(h => h.Trim()).ToList();
        var missing = Columns.Where(c => !header.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
        if (missing.Count > 0)
            return new([], $"The header row is missing: {string.Join(", ", missing)}.");

        int Index(string column) => header.FindIndex(h => h.Equals(column, StringComparison.OrdinalIgnoreCase));
        var at = Columns.ToDictionary(c => c, Index);

        var rows = lines.Skip(1).Select(l =>
        {
            var cells = SplitLine(l.Line);
            string Cell(string column) => at[column] < cells.Count ? cells[at[column]].Trim() : string.Empty;
            return new StudentCsvRow(l.Row, Cell("StudentNo"), Cell("FirstName"), Cell("LastName"), Cell("Email"),
                Cell("Programme"), Cell("AcademicYear"), Cell("Batch"));
        }).ToList();

        return new(rows, null);
    }

    /// <summary>Splits one CSV line, honouring double quotes and "" as an escaped quote.</summary>
    public static List<string> SplitLine(string line)
    {
        var cells = new List<string>();
        var cell = new StringBuilder();
        var quoted = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"') { cell.Append('"'); i++; }
                else if (c == '"') quoted = false;
                else cell.Append(c);
            }
            else if (c == '"') quoted = true;
            else if (c == ',') { cells.Add(cell.ToString()); cell.Clear(); }
            else cell.Append(c);
        }

        cells.Add(cell.ToString());
        return cells;
    }
}
