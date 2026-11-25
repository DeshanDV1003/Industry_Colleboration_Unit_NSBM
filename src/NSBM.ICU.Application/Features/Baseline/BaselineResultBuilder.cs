using NSBM.ICU.Application.Features.Baseline.Dtos;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Baseline;

/// <summary>Builds the result view of an attempt: per-skill snapshot plus a question-by-question review.</summary>
internal static class BaselineResultBuilder
{
    /// <param name="questions">The path's questions with <c>Options</c> and <c>Skill</c> loaded.</param>
    public static BaselineResultDto Build(BaselineAttempt attempt, IReadOnlyCollection<BaselineQuestion> questions)
    {
        var questionById = questions.ToDictionary(q => q.Id);
        var answered = attempt.Answers
            .Where(a => questionById.ContainsKey(a.QuestionId))
            .Select(a => (Answer: a, Question: questionById[a.QuestionId]))
            .OrderBy(x => x.Question.SortOrder)
            .ToList();

        var skills = answered
            .GroupBy(x => (x.Question.SkillId, x.Question.Skill!.Name))
            .Select(g =>
            {
                var correct = g.Count(x => x.Answer.IsCorrect);
                var total = g.Count();
                return new SkillSnapshotDto(g.Key.SkillId, g.Key.Name, correct, total, SkillSignalRules.Classify(correct, total));
            })
            .OrderBy(s => s.Signal).ThenBy(s => s.SkillName)
            .ToList();

        var review = answered
            .Select(x => new QuestionReviewDto(
                x.Question.Id,
                x.Question.Prompt,
                x.Question.Skill!.Name,
                x.Answer.IsCorrect,
                SelectedOption: x.Question.Options.FirstOrDefault(o => o.Id == x.Answer.SelectedOptionId)?.Text ?? string.Empty,
                CorrectOption: x.Question.Options.First(o => o.IsCorrect).Text,
                x.Question.Explanation))
            .ToList();

        return new BaselineResultDto(attempt.Id, attempt.CompletedAt, attempt.CorrectCount, attempt.QuestionCount,
            SkillSignalRules.Percent(attempt.CorrectCount, attempt.QuestionCount), skills, review);
    }
}
