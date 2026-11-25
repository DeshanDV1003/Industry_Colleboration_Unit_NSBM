using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.AiInterviews.Queries.GetAiInterviewAvailability;

/// <summary>Whether the AI interviewer is configured, so the page can explain instead of failing.</summary>
public sealed record GetAiInterviewAvailabilityQuery : IRequest<AiInterviewAvailabilityDto>;

public sealed class GetAiInterviewAvailabilityQueryHandler(IAiInterviewer interviewer) : IRequestHandler<GetAiInterviewAvailabilityQuery, AiInterviewAvailabilityDto>
{
    public Task<AiInterviewAvailabilityDto> Handle(GetAiInterviewAvailabilityQuery request, CancellationToken ct) =>
        Task.FromResult(new AiInterviewAvailabilityDto(interviewer.IsAvailable, AiInterviewRubric.QuestionsPerSession));
}
