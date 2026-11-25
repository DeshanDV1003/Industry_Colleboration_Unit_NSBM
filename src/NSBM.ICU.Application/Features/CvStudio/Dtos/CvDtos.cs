namespace NSBM.ICU.Application.Features.CvStudio.Dtos;

/// <summary>The student's CV outline: editable fields plus profile facts and listed skills.</summary>
public sealed record CvDto(
    string FullName,
    string Email,
    string StudentNo,
    string Programme,
    string? CareerPathName,
    string Headline,
    string Summary,
    string Education,
    string Projects,
    string? GitHubUrl,
    string? LinkedInUrl,
    IReadOnlyList<string> Skills);

/// <summary>A catalogue skill found in pasted CV text. Finding a word is not evidence of skill level.</summary>
public sealed record CvKeywordMatchDto(int SkillId, string Name, bool AlreadyListed);
