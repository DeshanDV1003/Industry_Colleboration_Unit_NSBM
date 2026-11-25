using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Baseline.Dtos;

/// <summary>The baseline for the student's chosen path. <see cref="CareerPathId"/> is null until a path is chosen.</summary>
public sealed record BaselineDto(
    int? CareerPathId,
    string? CareerPathName,
    IReadOnlyList<BaselineQuestionDto> Questions,
    BaselineResultDto? LatestResult);

public sealed record BaselineQuestionDto(int Id, string Prompt, string SkillName, IReadOnlyList<BaselineOptionDto> Options);

public sealed record BaselineOptionDto(int Id, string Text);

public sealed record BaselineResultDto(
    int AttemptId,
    DateTime CompletedAt,
    int CorrectCount,
    int QuestionCount,
    int ScorePercent,
    IReadOnlyList<SkillSnapshotDto> Skills,
    IReadOnlyList<QuestionReviewDto> Review);

public sealed record SkillSnapshotDto(int SkillId, string SkillName, int Correct, int Total, SkillSignal Signal);

public sealed record QuestionReviewDto(int QuestionId, string Prompt, string SkillName, bool IsCorrect, string SelectedOption, string CorrectOption, string? Explanation);
