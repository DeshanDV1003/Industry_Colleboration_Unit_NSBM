using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Mentoring;

namespace NSBM.ICU.Application.Features.Mentoring.Commands.AskMentor;

/// <summary>Adds a question to the mentor queue, where an industry partner can reply.</summary>
public sealed record AskMentorCommand(string Title, string Body) : IRequest<int>;

public sealed class AskMentorCommandValidator : AbstractValidator<AskMentorCommand>
{
    public AskMentorCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MinimumLength(20).WithMessage("Describe your question in at least 20 characters.").MaximumLength(4000);
    }
}

public sealed class AskMentorCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<AskMentorCommand, int>
{
    public async Task<int> Handle(AskMentorCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var question = new MentorQuestion
        {
            StudentId = student.Id,
            CareerPathId = student.CareerPathId,
            Title = request.Title.Trim(),
            Body = request.Body.Trim(),
            AskedAt = clock.UtcNow,
        };
        db.MentorQuestions.Add(question);
        await db.SaveChangesAsync(cancellationToken);
        return question.Id;
    }
}
