namespace NSBM.ICU.Application.Common.Exceptions;

/// <summary>Raised when a request is well-formed but violates a business rule.</summary>
public sealed class BusinessRuleException(string message) : Exception(message);
