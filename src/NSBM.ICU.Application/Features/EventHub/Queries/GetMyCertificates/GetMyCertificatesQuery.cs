using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.EventHub.Dtos;

namespace NSBM.ICU.Application.Features.EventHub.Queries.GetMyCertificates;

/// <summary>The signed-in student's participation certificates, newest first.</summary>
public sealed record GetMyCertificatesQuery : IRequest<IReadOnlyList<CertificateDto>>;

public sealed class GetMyCertificatesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyCertificatesQuery, IReadOnlyList<CertificateDto>>
{
    public async Task<IReadOnlyList<CertificateDto>> Handle(GetMyCertificatesQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        return await db.EventRegistrations.AsNoTracking().Where(r => r.StudentId == student.Id).ToCertificates(db).ToListAsync(ct);
    }
}
