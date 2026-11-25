namespace NSBM.ICU.Application.Common.Exceptions;

/// <summary>The AI service is not configured or could not answer. The API responds 503 so the student can retry.</summary>
public sealed class AiServiceUnavailableException(string message, Exception? inner = null) : Exception(message, inner);
