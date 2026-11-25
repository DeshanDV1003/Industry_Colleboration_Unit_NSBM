namespace NSBM.ICU.Application.Features.Mentoring.Dtos;

public sealed record MentorQuestionDto(
    int Id,
    string Title,
    string Body,
    string? CareerPathName,
    string StudentName,
    DateTime AskedAt,
    string? Reply,
    string? RepliedBy,
    string? RepliedByCompany,
    DateTime? RepliedAt);
