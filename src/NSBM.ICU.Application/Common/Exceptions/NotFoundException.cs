namespace NSBM.ICU.Application.Common.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
    public static NotFoundException For(string entity, object key) => new($"{entity} '{key}' was not found.");
}
