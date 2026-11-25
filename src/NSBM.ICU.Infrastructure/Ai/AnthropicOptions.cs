namespace NSBM.ICU.Infrastructure.Ai;

/// <summary>
/// Settings for the Claude-powered AI interviewer ("Anthropic" section). Never put the key in appsettings.json:
/// set it with <c>dotnet user-secrets set "Anthropic:ApiKey" "..."</c> or the ANTHROPIC_API_KEY environment variable.
/// Without a key the AI interviewer is simply reported as unavailable.
/// </summary>
public sealed class AnthropicOptions
{
    public const string SectionName = "Anthropic";

    public string? ApiKey { get; set; }
    public string Model { get; set; } = "claude-opus-5-5";
    /// <summary>Upper bound for one interviewer reply, including the model's internal thinking.</summary>
    public int QuestionMaxTokens { get; set; } = 4000;
    public int FeedbackMaxTokens { get; set; } = 16000;
}
