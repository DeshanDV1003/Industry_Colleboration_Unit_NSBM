using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Planner.Dtos;

public sealed record PlannerItemDto(int Id, PlannerItemKind Kind, string Title, string? Notes, DateOnly? DueDate, bool IsDone, DateTime? CompletedAt);
