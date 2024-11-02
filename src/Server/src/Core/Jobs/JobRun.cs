namespace Anyding.Jobs;

public class JobRun : Entity<Guid>
{
    public Guid? DefinitionId { get; set; }
    public JobType Type { get; set; }
    public string Details { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
    public JobStatus Status { get; set; }

    public ICollection<JobRunStep> Steps { get; set; }
}

public class JobRunStep: Entity<Guid>
{
    public string? Description { get; set; }
    public Guid JobRunId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public JobStatus Status { get; set; }

    public IDictionary<string, object> Properties { get; set; }
}

public enum JobType
{
    Discovery
}

public enum JobStatus
{
    Created,
    Running,
    Completed,
    Failed
}
