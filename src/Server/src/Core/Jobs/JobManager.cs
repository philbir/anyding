using System.Text.Json;
using Anyding.Data;
using Anyding.Jobs.Command;
using MediatR;

namespace Anyding.Jobs;

public class JobManager(JobFactory jobFactory, IMediator mediator)
{
    public async Task RunAsync(
        JobDefintion definition,
        CancellationToken ct)
    {
        JobContext context = new() { Defintion = definition };

        var jobRun = new JobRun
        {
            Id = Guid.NewGuid(),
            Type = definition.Type,
            DefinitionId = definition.Id != Guid.Empty ? definition.Id : null,
            Status= JobStatus.Created,
            Details = JsonSerializer.Serialize(definition.Details),
            CreatedAt = DateTime.UtcNow
        };

        await mediator.Send(new NewJobCommand(jobRun), ct);
        context.JobId = jobRun.Id;

        IJob job = jobFactory.Create(definition.Type);
        await job.RunAsync(context, ct);
    }
}

public interface IJobDetails
{
}

public class JobDefintion : Entity<Guid>
{
    public JobType Type { get; set; }
    public string Name { get; set; }

    public string Details { get; set; }

    public bool Enabled { get; set; }
    public JobSchedule? Schedule { get; set; }
}

public class JobSchedule
{
    public JobScheduleType Type { get; set; }

    public string? CronExpression { get; set; }

    /// <summary>
    /// Intervall in seconds
    /// </summary>
    public int? Interval { get; set; }
}

public enum JobScheduleType
{
    Interval,
    Cron
}

public class JobFactory
{
    private readonly IEnumerable<IJob> _jobs;

    public JobFactory(IEnumerable<IJob> jobs)
    {
        _jobs = jobs;
    }

    public IJob Create(JobType type)
    {
        return _jobs.First(x => x.Type == type);
    }
}

public interface IJob
{
    Task RunAsync(
        JobContext context,
        CancellationToken ct);

    JobType Type { get; }
}

public class JobContext
{
    public Guid JobId { get; set; }
    public JobDefintion Defintion { get; set; }
}
