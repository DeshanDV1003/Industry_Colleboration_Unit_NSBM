namespace NSBM.ICU.Domain.Constants;

/// <summary>Platform roles. Values are used verbatim as Identity role names and in [Authorize] attributes.</summary>
public static class Roles
{
    public const string Student = nameof(Student);
    public const string Lecturer = nameof(Lecturer);
    public const string Industry = nameof(Industry);
    public const string Admin = nameof(Admin);
    /// <summary>The Assistant Registrar: allocates lecture halls and event halls.</summary>
    public const string Registrar = nameof(Registrar);

    public static readonly IReadOnlyList<string> All = [Student, Lecturer, Industry, Admin, Registrar];
}
