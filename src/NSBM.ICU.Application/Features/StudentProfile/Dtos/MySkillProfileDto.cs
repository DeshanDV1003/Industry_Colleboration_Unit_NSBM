namespace NSBM.ICU.Application.Features.StudentProfile.Dtos;

/// <summary>The skills a student lists (self-reported) and whether partners may view their profile.</summary>
public sealed record MySkillProfileDto(IReadOnlyList<int> SkillIds, bool ShareProfileWithPartners);
