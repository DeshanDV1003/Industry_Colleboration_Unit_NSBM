using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>
/// The AI practice interviewer. Implemented in Infrastructure; the application never depends on a specific AI SDK.
/// Methods throw <see cref="Exceptions.AiServiceUnavailableException"/> when the service is not configured or fails.
/// </summary>
public interface IAiInterviewer
{
    /// <summary>False when no API key is configured; the feature then explains this instead of failing.</summary>
    bool IsAvailable { get; }

    /// <summary>The interviewer's next message: an opening question, a follow-up or a new question.</summary>
    Task<string> NextQuestionAsync(AiInterviewBrief brief, IReadOnlyList<AiTranscriptLine> transcript, CancellationToken ct);

    /// <summary>Practice feedback on the whole transcript, scored against <see cref="AiInterviewBrief.Criteria"/>.</summary>
    Task<AiInterviewEvaluation> EvaluateAsync(AiInterviewBrief brief, IReadOnlyList<AiTranscriptLine> transcript, CancellationToken ct);
}
