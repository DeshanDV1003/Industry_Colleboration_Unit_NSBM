using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>
/// Sends reset links in the background. Queuing (instead of sending during the request) keeps the forgot-password
/// response equally fast whether or not the account exists, so the timing does not reveal who has an account.
/// </summary>
public interface IPasswordResetMailer
{
    bool IsAvailable { get; }
    int LinkValidMinutes { get; }

    void Enqueue(PasswordResetTicket ticket);
}
