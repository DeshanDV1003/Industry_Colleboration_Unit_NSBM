using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryTasks.Dtos;

namespace NSBM.ICU.Application.Features.IndustryTasks.Queries.GetIndustryTask;

/// <summary>The complete task, including pattern checks. Visible to the authoring company and to Unit admins.</summary>
public sealed record GetIndustryTaskQuery(int Id) : IRequest<IndustryTaskDto>;

public sealed class GetIndustryTaskQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetIndustryTaskQuery, IndustryTaskDto>
{
    public async Task<IndustryTaskDto> Handle(GetIndustryTaskQuery request, CancellationToken cancellationToken)
    {
        var companyId = await db.IndustryTasks.Where(t => t.Id == request.Id).Select(t => (int?)t.CompanyId).FirstOrDefaultAsync(cancellationToken)
                        ?? throw NotFoundException.For("Task", request.Id);

        await db.EnsureCompanyAccessAsync(currentUser, companyId, cancellationToken);

        return (await db.IndustryTasks.Where(t => t.Id == request.Id).ToDetailAsync(cancellationToken))!;
    }
}
