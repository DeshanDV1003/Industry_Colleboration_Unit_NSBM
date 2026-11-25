using FluentValidation;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.IndustryTasks.Models;

public sealed record TaskCheckInput(string Description, string Pattern, bool ShouldMatch, string? Hint);

/// <summary>Everything a partner provides when creating or editing an industry task.</summary>
public sealed record IndustryTaskInput(
    string Title,
    string Summary,
    string Instructions,
    int CareerPathId,
    LearningStage Level,
    WorkSituation Situation,
    int EstimatedMinutes,
    string? StarterLanguage,
    string? StarterCode,
    IReadOnlyList<int> SkillIds,
    IReadOnlyList<TaskCheckInput> Checks,
    IReadOnlyList<string> KnowledgeQuestions);

public sealed class IndustryTaskInputValidator : AbstractValidator<IndustryTaskInput>
{
    public IndustryTaskInputValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Summary).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Instructions).NotEmpty().MaximumLength(8000);
        RuleFor(x => x.CareerPathId).GreaterThan(0).WithMessage("Choose the job role this task belongs to.");
        RuleFor(x => x.Level).IsInEnum();
        RuleFor(x => x.Situation).IsInEnum();
        RuleFor(x => x.EstimatedMinutes).InclusiveBetween(5, 480);
        RuleFor(x => x.StarterLanguage).MaximumLength(30);
        RuleFor(x => x.StarterCode).MaximumLength(20000);

        RuleFor(x => x.SkillIds).NotEmpty().WithMessage("List at least one skill or tool the task needs.");
        RuleFor(x => x.SkillIds).Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Each skill can be listed once.");

        RuleFor(x => x.Checks).NotEmpty().WithMessage("Add at least one pattern check so students get feedback.");
        RuleForEach(x => x.Checks).ChildRules(check =>
        {
            check.RuleFor(c => c.Description).NotEmpty().MaximumLength(300);
            check.RuleFor(c => c.Pattern).NotEmpty().MaximumLength(500)
                .Must(PatternCheckRules.IsValidPattern).WithMessage("'{PropertyValue}' is not a valid regular expression.");
            check.RuleFor(c => c.Hint).MaximumLength(500);
        });

        RuleFor(x => x.KnowledgeQuestions).NotEmpty().WithMessage("Add at least one follow-up question.");
        RuleForEach(x => x.KnowledgeQuestions).NotEmpty().MaximumLength(500);
    }
}
