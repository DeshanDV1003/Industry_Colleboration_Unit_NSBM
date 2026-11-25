using System.Security.Cryptography;

namespace NSBM.ICU.Domain.Rules;

/// <summary>Short, unguessable codes people type or share: team join codes and certificate numbers.</summary>
public static class ShareCodeRules
{
    // No 0/O or 1/I, so codes read back without mistakes.
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public const int TeamJoinCodeLength = 6;
    public const int CertificateCodeLength = 10;
    public const string CertificatePrefix = "ICU-";
    public const string BadgePrefix = "BDG-";
    public const int BadgeCodeLength = 8;
    public const int ProfileSlugLength = 12;

    public static string NewTeamJoinCode() => RandomNumberGenerator.GetString(Alphabet, TeamJoinCodeLength);

    public static string NewCertificateCode() => CertificatePrefix + RandomNumberGenerator.GetString(Alphabet, CertificateCodeLength);

    public static string NewBadgeCode() => BadgePrefix + RandomNumberGenerator.GetString(Alphabet, BadgeCodeLength);

    /// <summary>Lower-case so profile links read naturally in a URL.</summary>
    public static string NewProfileSlug() => RandomNumberGenerator.GetString(Alphabet, ProfileSlugLength).ToLowerInvariant();

    public static string Normalise(string code) => code.Trim().ToUpperInvariant();
}
