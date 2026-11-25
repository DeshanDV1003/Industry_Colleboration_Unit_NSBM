using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Moderation;

/// <summary>An administrator's review of one piece of partner content.</summary>
public interface IReviewCommand
{
    int Id { get; }
    ReviewDecision Decision { get; }
    string? Note { get; }
}
