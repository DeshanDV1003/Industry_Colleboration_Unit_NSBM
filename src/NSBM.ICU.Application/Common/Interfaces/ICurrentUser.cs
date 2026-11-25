namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>The account making the current request.</summary>
public interface ICurrentUser
{
    string? UserId { get; }
    string? Role { get; }
}
