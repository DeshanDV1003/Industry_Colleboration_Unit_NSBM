using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Careers.Dtos;

public sealed record SkillDto(int Id, string Name, SkillCategory Category);
