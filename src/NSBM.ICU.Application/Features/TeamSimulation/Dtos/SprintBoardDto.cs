using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.TeamSimulation.Dtos;

/// <summary>
/// One card on the shared board. Cards in the student's role can be picked up and moved; the rest are owned by
/// fictional teammates.
/// </summary>
public sealed record SprintItemDto(
    int Id,
    string Key,
    string Title,
    string Description,
    int StoryPoints,
    string RoleName,
    string Owner,
    SprintItemStatus Status,
    bool IsMine,
    bool CanPickUp);

public sealed record SprintBoardDto(string? MyRoleName, IReadOnlyList<SprintItemDto> Items);
