namespace NSBM.ICU.Application.Common.Exceptions;

public sealed class AuthenticationFailedException(string message = "Invalid email or password.")
    : Exception(message);
