namespace NSBM.ICU.Domain.Enums;

/// <summary>How an answer was entered. Voice answers are transcribed by the browser; no audio is stored.</summary>
public enum AnswerInputMode
{
    Typed = 1,
    Voice = 2,
}
