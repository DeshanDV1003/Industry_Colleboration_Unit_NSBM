namespace NSBM.ICU.Infrastructure.Identity;

/// <summary>Controls demo-only behaviour such as the one-click role switcher.</summary>
public sealed class DemoOptions
{
    public const string SectionName = "Demo";

    public bool Enabled { get; init; }
}
