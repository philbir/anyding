using Anyding.Data;
using MediatR;

namespace Anyding.Jobs.Command;

public record AddJobStepCommand(Guid JobRunId, string Description) : IRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public IDictionary<string, object> Properties { get; init; } = new Dictionary<string, object>();
}

public class AddJobStepCommandHandler(AnydingDbContext db) : IRequestHandler<AddJobStepCommand>
{
    public async Task Handle(AddJobStepCommand request, CancellationToken ct)
    {
        var jobRunStep = new JobRunStep
        {
            Id = request.Id,
            JobRunId = request.JobRunId,
            Description = request.Description,
            StartedAt = DateTime.UtcNow,
            Status = JobStatus.Created,
            Properties = request.Properties
        };

        await db.JobRunSteps.AddAsync(jobRunStep, ct);
        await db.SaveChangesAsync(ct);
    }
}
