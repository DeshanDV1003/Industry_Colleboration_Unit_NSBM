namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>Demo-only maintenance of the fictional sample data.</summary>
public interface IDemoDataService
{
    bool IsEnabled { get; }

    /// <summary>Removes all activity and partner content, then reloads the original demo data. Accounts are kept.</summary>
    Task ResetAsync(CancellationToken cancellationToken);
}
