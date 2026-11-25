using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.EventHub.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.EventHub.Queries.VerifyCertificate;

/// <summary>
/// Public check that a certificate is genuine, for employers a student shares it with. It returns only
/// what is printed on the certificate itself.
/// </summary>
public sealed record VerifyCertificateQuery(string Code) : IRequest<CertificateDto>;

public sealed class VerifyCertificateQueryValidator : AbstractValidator<VerifyCertificateQuery>
{
    public VerifyCertificateQueryValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
}

public sealed class VerifyCertificateQueryHandler(IApplicationDbContext db) : IRequestHandler<VerifyCertificateQuery, CertificateDto>
{
    public async Task<CertificateDto> Handle(VerifyCertificateQuery request, CancellationToken ct)
    {
        var code = ShareCodeRules.Normalise(request.Code);
        return await db.EventRegistrations.AsNoTracking().Where(r => r.CertificateCode == code).ToCertificates(db).FirstOrDefaultAsync(ct)
               ?? throw new NotFoundException("No certificate with this code exists.");
    }
}
