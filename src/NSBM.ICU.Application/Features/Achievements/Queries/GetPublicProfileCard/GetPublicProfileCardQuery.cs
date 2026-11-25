using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Achievements.Queries.GetPublicProfileCard;

/// <summary>
/// The public profile card behind a shared link. Not found unless the student has sharing turned on, so a
/// turned-off card looks the same as one that never existed.
/// </summary>
public sealed record GetPublicProfileCardQuery(string Slug) : IRequest<ProfileCardDto>;

public sealed class GetPublicProfileCardQueryValidator : AbstractValidator<GetPublicProfileCardQuery>
{
    public GetPublicProfileCardQueryValidator() => RuleFor(x => x.Slug).NotEmpty().MaximumLength(32);
}

public sealed class GetPublicProfileCardQueryHandler(IApplicationDbContext db, IDateTime clock) : IRequestHandler<GetPublicProfileCardQuery, ProfileCardDto>
{
    public async Task<ProfileCardDto> Handle(GetPublicProfileCardQuery request, CancellationToken ct)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();
        var student = await db.Students.AsNoTracking().Include(s => s.CareerPath)
                          .FirstOrDefaultAsync(s => s.ShareProfileCard && s.ProfileCardSlug == slug, ct)
                      ?? throw new NotFoundException("This profile card is not available.");

        var progress = await db.LoadProgressAsync(student, DateOnly.FromDateTime(clock.UtcNow), ct);
        var badges = await db.SkillBadges.AsNoTracking()
            .Where(b => b.StudentId == student.Id && b.Status == ModerationStatus.Approved)
            .OrderByDescending(b => b.ReviewedAt)
            .Select(b => new CardBadgeDto(b.Skill!.Name, b.VerificationCode!, b.ReviewedAt!.Value))
            .ToListAsync(ct);
        var certificates = await db.EventRegistrations.AsNoTracking()
            .Where(r => r.StudentId == student.Id && r.CertificateCode != null)
            .Join(db.IndustryEvents, r => r.EventId, e => e.Id, (r, e) => new { r, e })
            .OrderByDescending(x => x.r.CertificateIssuedAt)
            .Select(x => new CardCertificateDto(x.e.Title, x.e.Company!.Name, x.r.CertificateCode!, x.r.CertificateIssuedAt!.Value))
            .ToListAsync(ct);

        return new ProfileCardDto(student.FullName, student.Programme, student.CareerPath?.Name, progress.Level,
            progress.CurrentStreak, progress.BestStreak, badges, certificates);
    }
}
