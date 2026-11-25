namespace NSBM.ICU.Domain.Common;

/// <summary>Root type for every persisted domain entity.</summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
}
