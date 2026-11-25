using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Support;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Lecturing.Commands.AddSupportNote;

/// <summary>Saves an academic feedback note. The student sees it in their portfolio.</summary>
public sealed record AddSupportNoteCommand(int StudentId, string Text) : IRequest<int>;

public sealed class AddSupportNoteCommandValidator : AbstractValidator<AddSupportNoteCommand>
{
    public AddSupportNoteCommandValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.Text).NotEmpty().MinimumLength(10).WithMessage("Write a note of at least 10 characters.").MaximumLength(2000);
    }
}

public sealed class AddSupportNoteCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<AddSupportNoteCommand, int>
{
    public async Task<int> Handle(AddSupportNoteCommand request, CancellationToken ct)
    {
        var lecturer = await db.GetCurrentLecturerAsync(currentUser, ct);
        if (!await db.Students.AnyAsync(s => s.Id == request.StudentId, ct))
            throw NotFoundException.For("Student", request.StudentId);

        var note = new SupportNote
        {
            StudentId = request.StudentId,
            LecturerId = lecturer.Id,
            LecturerName = lecturer.FullName,
            Text = request.Text.Trim(),
            CreatedAt = clock.UtcNow,
        };
        db.SupportNotes.Add(note);
        db.Notify(await db.StudentUserIdAsync(request.StudentId, ct), NotificationKind.LecturerNote,
            "New feedback from your lecturer", $"{lecturer.FullName} left you a note.", NotificationLinks.Portfolio, clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return note.Id;
    }
}
