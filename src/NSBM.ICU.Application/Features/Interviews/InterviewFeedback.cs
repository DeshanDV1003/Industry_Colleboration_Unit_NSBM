using NSBM.ICU.Application.Features.Interviews.Dtos;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Interviews;

internal static class InterviewFeedback
{
    private const char Separator = ';';

    public static string JoinKeywords(IEnumerable<string> keywords) => string.Join(Separator, keywords);

    public static AnswerFeedbackDto Build(InterviewAnswer answer, InterviewQuestion question)
    {
        var all = question.KeywordList;
        var matched = answer.MatchedKeywords.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        var missing = all.Except(matched, StringComparer.OrdinalIgnoreCase).ToList();

        return new AnswerFeedbackDto(
            answer.Id,
            answer.Text,
            answer.InputMode,
            answer.AnsweredAt,
            answer.WordCount,
            matched,
            missing,
            SkillSignalRules.Percent(matched.Length, all.Count),
            InterviewFeedbackRules.Notes(answer.WordCount, matched.Length, all.Count));
    }
}
