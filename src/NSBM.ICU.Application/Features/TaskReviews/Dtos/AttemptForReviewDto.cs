using NSBM.ICU.Application.Features.Practice.Dtos;

namespace NSBM.ICU.Application.Features.TaskReviews.Dtos;

/// <summary>A student's latest attempt on one of the company's tasks, ready for a review comment.</summary>
public sealed record AttemptForReviewDto(
    int AttemptId,
    int TaskId,
    string TaskTitle,
    string StudentName,
    string StudentNo,
    int AttemptNumber,
    DateTime SubmittedAt,
    int PassedChecks,
    int TotalChecks,
    string Code,
    IReadOnlyList<AttemptReviewDto> Reviews);
