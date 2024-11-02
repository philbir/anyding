using Anyding.Data;
using MediatR;

namespace Anyding.Jobs.Command;

public record NewJobCommand(JobRun Job) : IRequest;

public class NewJobCommandHandler(AnydingDbContext db) : IRequestHandler<NewJobCommand>
{
    public async Task Handle(
        NewJobCommand request,
        CancellationToken cancellationToken)
    {
        await db.JobRuns.AddAsync(request.Job, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}

