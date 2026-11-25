namespace NSBM.ICU.Domain.Enums;

/// <summary>What the platform can show about a student and one skill. Practice is a signal, never proof.</summary>
public enum SkillEvidence
{
    /// <summary>Neither listed by the student nor seen in practice.</summary>
    Missing = 1,
    /// <summary>The student lists the skill, but there is no practice signal yet.</summary>
    ListedOnly = 2,
    /// <summary>A baseline answer or task activity suggests the skill is developing.</summary>
    PracticeSignal = 3,
}
