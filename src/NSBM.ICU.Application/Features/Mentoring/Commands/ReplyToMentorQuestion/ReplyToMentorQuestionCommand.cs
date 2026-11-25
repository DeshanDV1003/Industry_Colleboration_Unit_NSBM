using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Mentoring.Commands.ReplyToMentorQuestion;

/// <summary>An industry partner answers a student's question. Each question gets one mentor reply.</summary>
public sealed record ReplyToMentorQuestionCommand(int QuestionId, string Reply) : IRequest;

public sealed class ReplyToMentorQuestionCommandValidator : AbstractValidator<ReplyToMentorQuestionCommand>
{
    public ReplyToMentorQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionId).GreaterThan(0);
        RuleFor(x => x.Reply).NotEmpty().MinimumLength(20).WithMessage("Write a helpful reply of at least 20 characters.").MaximumLength(4000);
    }
}

public sealed class ReplyToMentorQuestionCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<ReplyToMentorQuestionCommand>
{
    public async Task Handle(ReplyToMentorQuestionCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        var companyName = await db.Companies.Where(c => c.Id == partner.CompanyId).Select(c => c.Name).SingleAsync(cancellationToken);
        var question = await db.MentorQuestions.FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken)
                       ?? throw NotFoundException.For("Mentor question", request.QuestionId);

        if (question.IsAnswered)
            throw new BusinessRuleException("This question has already been answered by a mentor.");

        question.Answer(request.Reply.Trim(), partner.FullName, companyName, clock.UtcNow);
        db.Notify(await db.StudentUserIdAsync(question.StudentId, cancellationToken), NotificationKind.MentorReply,
            $"A mentor answered: {question.Title}", $"{partner.FullName} from {companyName} replied to your question.",
            NotificationLinks.MentorCenter, clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
    }
}
