using NSBM.ICU.Application.Features.Users;
using NSBM.ICU.Application.Features.Users.Commands.ImportStudents;

namespace NSBM.ICU.UnitTests.Users;

public sealed class StudentCsvTests
{
    private const string Header = "StudentNo,FirstName,LastName,Email,Programme,AcademicYear,Batch";

    [Fact]
    public void Reads_rows_under_the_header()
    {
        var result = StudentCsv.Parse($"{Header}\nS1,Ayesha,Fonseka,a@x.lk,BSc SE,1,2026.1\nS2,Kasun,R,k@x.lk,BSc CS,2,2025.1");
        Assert.Null(result.Error);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal(new StudentCsvRow(2, "S1", "Ayesha", "Fonseka", "a@x.lk", "BSc SE", "1", "2026.1"), result.Rows[0]);
        Assert.Equal(3, result.Rows[1].Row);
    }

    [Fact]
    public void Columns_can_come_in_any_order_and_case()
    {
        var result = StudentCsv.Parse("email,STUDENTNO,batch,academicyear,programme,lastname,firstname\na@x.lk,S1,2026.1,1,BSc SE,Fonseka,Ayesha");
        Assert.Equal("S1", result.Rows[0].StudentNo);
        Assert.Equal("Ayesha", result.Rows[0].FirstName);
    }

    [Fact]
    public void Quoted_values_keep_commas_and_escaped_quotes() =>
        Assert.Equal(["BSc (Hons), \"Top\" SE", "1"], StudentCsv.SplitLine("\"BSc (Hons), \"\"Top\"\" SE\",1"));

    [Fact]
    public void Windows_line_endings_and_blank_lines_are_fine()
    {
        var result = StudentCsv.Parse($"{Header}\r\n\r\nS1,A,B,a@x.lk,P,1,2026.1\r\n");
        Assert.Single(result.Rows);
        Assert.Equal(3, result.Rows[0].Row);
    }

    [Fact]
    public void Missing_columns_are_named()
    {
        var result = StudentCsv.Parse("StudentNo,FirstName,Email\nS1,A,a@x.lk");
        Assert.Contains("LastName", result.Error);
        Assert.Contains("Batch", result.Error);
    }

    [Fact]
    public void Empty_text_is_an_error() => Assert.NotNull(StudentCsv.Parse("  \n ").Error);

    [Theory]
    [InlineData("S1", "a@x.lk", "1", true)]
    [InlineData("S1", "not-an-email", "1", false)]
    [InlineData("S1", "a@x.lk", "nine", false)]
    [InlineData("S1", "a@x.lk", "6", false)]
    [InlineData("", "a@x.lk", "1", false)]
    public void Rows_get_the_single_account_rules(string studentNo, string email, string year, bool valid) =>
        Assert.Equal(valid, new StudentCsvRowValidator().Validate(new StudentCsvRow(2, studentNo, "A", "B", email, "BSc SE", year, "2026.1")).IsValid);
}
