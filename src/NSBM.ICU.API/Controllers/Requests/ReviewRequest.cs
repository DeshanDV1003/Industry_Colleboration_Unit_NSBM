using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.API.Controllers.Requests;

/// <summary>Body of every moderation review endpoint.</summary>
public sealed record ReviewRequest(ReviewDecision Decision, string? Note);
