namespace NSBM.ICU.Domain.Common;

/// <summary>Thrown by an entity when an action would break one of its rules. The API answers 400 with the message.</summary>
public sealed class DomainRuleException(string message) : Exception(message);
