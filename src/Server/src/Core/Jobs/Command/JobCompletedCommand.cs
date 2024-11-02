using Anyding.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Jobs.Command;

public record JobCompletedCommand(Guid JobId, JobStatus Status) : IRequest;

public class JobCompletedCommandHandler(IAnydingDbContext dbContext) : IRequestHandler<JobCompletedCommand>
{
    public async Task Handle(JobCompletedCommand request, CancellationToken ct)
    {
        await dbContext.JobRuns.Where(x => x.Id == request.JobId)
            .ExecuteUpdateAsync(update => update
                .SetProperty(j => j.Status, request.Status)
                .SetProperty(j => j.CompletedAt, DateTime.UtcNow),
                ct);
    }
}
