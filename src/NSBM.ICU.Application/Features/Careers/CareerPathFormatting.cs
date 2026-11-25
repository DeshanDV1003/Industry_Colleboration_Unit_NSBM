namespace NSBM.ICU.Application.Features.Careers;

internal static class CareerPathFormatting
{
    private const char RoleSeparator = ';';

    public static IReadOnlyList<string> SplitEntryRoles(string entryRoles) =>
        entryRoles.Split(RoleSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
