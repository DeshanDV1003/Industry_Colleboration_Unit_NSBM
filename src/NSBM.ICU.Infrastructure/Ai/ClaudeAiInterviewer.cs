using System.Text.Json;
using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Beta;
using Anthropic.Models.Beta.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Infrastructure.Ai;

/// <summary>
/// The AI practice interviewer, backed by Claude. Only the typed interview text is sent; never audio or video.
/// Uses the beta Messages endpoint so a policy refusal is re-served by Anthropic's server-side fallback.
/// </summary>
public sealed class ClaudeAiInterviewer : IAiInterviewer
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly AnthropicOptions _options;
    private readonly ILogger<ClaudeAiInterviewer> _logger;
    private readonly AnthropicClient? _client;

    public ClaudeAiInterviewer(IOptions<AnthropicOptions> options, ILogger<ClaudeAiInterviewer> logger)
    {
        _options = options.Value;
        _logger = logger;
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            _client = new AnthropicClient { ApiKey = _options.ApiKey };
    }

    public bool IsAvailable => _client is not null;

    public async Task<string> NextQuestionAsync(AiInterviewBrief brief, IReadOnlyList<AiTranscriptLine> transcript, CancellationToken ct)
    {
        var messages = new List<BetaMessageParam> { new BetaUserMessageParam { Content = AiInterviewPrompts.Kickoff(brief) } };
        for (var i = 0; i < transcript.Count; i++)
        {
            var line = transcript[i];
            if (line.Speaker == AiTurnSpeaker.Interviewer)
            {
                messages.Add(new BetaAssistantMessageParam { Content = line.Text });
                continue;
            }

            // The progress note rides on the newest answer only, as its own text block after the student's words.
            var isLatest = i == transcript.Count - 1;
            messages.Add(new BetaUserMessageParam
            {
                Content = isLatest
                    ? new List<BetaContentBlockParam>
                    {
                        new BetaTextBlockParam { Text = line.Text },
                        new BetaTextBlockParam { Text = AiInterviewPrompts.ProgressNote(brief) },
                    }
                    : line.Text,
            });
        }

        var response = await SendAsync(new MessageCreateParams
        {
            Model = _options.Model,
            MaxTokens = _options.QuestionMaxTokens,
            System = AiInterviewPrompts.InterviewerSystem,
            Messages = messages,
            OutputConfig = new BetaOutputConfig { Effort = Effort.Low },
            Betas = [AnthropicBeta.ServerSideFallback2026_07_01],
            Fallbacks = new Default(),
        }, ct);

        var text = TextOf(response);
        if (string.IsNullOrWhiteSpace(text))
            throw new AiServiceUnavailableException("The AI interviewer did not reply. Please try again.");
        return text;
    }

    public async Task<AiInterviewEvaluation> EvaluateAsync(AiInterviewBrief brief, IReadOnlyList<AiTranscriptLine> transcript, CancellationToken ct)
    {
        var schema = JsonSerializer.SerializeToElement(AiInterviewPrompts.FeedbackSchema(brief.Criteria))
            .EnumerateObject().ToDictionary(p => p.Name, p => p.Value.Clone());

        var response = await SendAsync(new MessageCreateParams
        {
            Model = _options.Model,
            MaxTokens = _options.FeedbackMaxTokens,
            System = AiInterviewPrompts.EvaluatorSystem,
            Messages = [new BetaUserMessageParam { Content = AiInterviewPrompts.EvaluationRequest(brief, transcript) }],
            OutputConfig = new BetaOutputConfig { Effort = Effort.Medium, Format = new BetaJsonOutputFormat { Schema = schema } },
            Betas = [AnthropicBeta.ServerSideFallback2026_07_01],
            Fallbacks = new Default(),
        }, ct);

        try
        {
            var feedback = JsonSerializer.Deserialize<FeedbackJson>(TextOf(response), Json)
                           ?? throw new JsonException("Empty feedback.");
            return new AiInterviewEvaluation(feedback.Summary, feedback.Strengths, feedback.Improvements,
                feedback.Scores.Select(s => new AiCriterionScore(s.Criterion, s.Score, s.Comment)).ToList());
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "AI interview feedback could not be parsed (stop reason {StopReason}).", response.StopReason);
            throw new AiServiceUnavailableException("The AI feedback could not be read. Please try finishing again.", ex);
        }
    }

    private async Task<BetaMessage> SendAsync(MessageCreateParams parameters, CancellationToken ct)
    {
        if (_client is null)
            throw new AiServiceUnavailableException("The AI interviewer is not set up yet.");

        BetaMessage response;
        try
        {
            response = await _client.Beta.Messages.Create(parameters, ct);
        }
        catch (AnthropicRateLimitException ex)
        {
            _logger.LogWarning(ex, "AI interviewer rate limited.");
            throw new AiServiceUnavailableException("The AI interviewer is busy right now. Please try again in a minute.", ex);
        }
        catch (Anthropic5xxException ex)
        {
            _logger.LogWarning(ex, "AI service error.");
            throw new AiServiceUnavailableException("The AI service had a problem. Please try again.", ex);
        }
        catch (AnthropicApiException ex)
        {
            // 4xx other than rate limits means a configuration problem (bad key, model name), not something to retry.
            _logger.LogError(ex, "AI interviewer request was rejected.");
            throw new AiServiceUnavailableException("The AI interviewer is not configured correctly. Please tell the Industry Collaboration Unit.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not reach the AI service.");
            throw new AiServiceUnavailableException("Could not reach the AI service. Check the connection and try again.", ex);
        }

        if (response.StopReason == "refusal")
            throw new AiServiceUnavailableException("The AI could not continue with that answer. Try rephrasing it.");
        return response;
    }

    private static string TextOf(BetaMessage response) =>
        string.Concat(response.Content.Select(b => b.TryPickText(out var t) ? t.Text : string.Empty)).Trim();

    private sealed record FeedbackJson(string Summary, List<string> Strengths, List<string> Improvements, List<ScoreJson> Scores);
    private sealed record ScoreJson(string Criterion, int Score, string Comment);
}
