using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Profiles;

namespace NSBM.ICU.Application.Features.CvStudio.Commands.SaveMyCv;

public sealed record SaveMyCvCommand(
    string Headline,
    string Summary,
    string Education,
    string Projects,
    string? GitHubUrl,
    string? LinkedInUrl) : IRequest;

public sealed class SaveMyCvCommandValidator : AbstractValidator<SaveMyCvCommand>
{
    public SaveMyCvCommandValidator()
    {
        RuleFor(x => x.Headline).MaximumLength(150);
        RuleFor(x => x.Summary).MaximumLength(2000);
        RuleFor(x => x.Education).MaximumLength(2000);
        RuleFor(x => x.Projects).MaximumLength(4000);
        RuleFor(x => x.GitHubUrl).Must(BeHttpsUrlOn("github.com")).WithMessage("Enter a GitHub link such as https://github.com/your-name.");
        RuleFor(x => x.LinkedInUrl).Must(BeHttpsUrlOn("linkedin.com")).WithMessage("Enter a LinkedIn link such as https://www.linkedin.com/in/your-name.");
    }

    private static Func<string?, bool> BeHttpsUrlOn(string host) => url =>
        string.IsNullOrWhiteSpace(url) ||
        (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps &&
         (uri.Host == host || uri.Host.EndsWith("." + host, StringComparison.OrdinalIgnoreCase)) && url.Length <= 300);
}

public sealed class SaveMyCvCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<SaveMyCvCommand>
{
    public async Task Handle(SaveMyCvCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var cv = await db.CvProfiles.FirstOrDefaultAsync(c => c.StudentId == student.Id, cancellationToken);
        if (cv is null)
        {
            cv = new CvProfile { StudentId = student.Id };
            db.CvProfiles.Add(cv);
        }

        cv.Headline = request.Headline.Trim();
        cv.Summary = request.Summary.Trim();
        cv.Education = request.Education.Trim();
        cv.Projects = request.Projects.Trim();
        cv.GitHubUrl = string.IsNullOrWhiteSpace(request.GitHubUrl) ? null : request.GitHubUrl.Trim();
        cv.LinkedInUrl = string.IsNullOrWhiteSpace(request.LinkedInUrl) ? null : request.LinkedInUrl.Trim();

        await db.SaveChangesAsync(cancellationToken);
    }
}
