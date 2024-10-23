using Anyding.Discovery;

namespace Anyding;

public interface IWorkspaceFactory
{
    string RootDirectory { get; }

    Task<IWorkspace> CreateNewWorkspaceAsync(
        Guid id,
        DiscoveredItem item,
        Stream stream,
        CancellationToken cancellationToken);

    Task<IWorkspace> GetWorkspaceAsync(Guid id, CancellationToken ct);
}

public class WorkspaceInfo
{
    public Guid Id { get; set; }
    public string ItemType { get; set; }
    public DiscoveredItem Discovery { get; set; }
    public List<TaskExecutionResult> PipelineResult { get; set; } = new();
    public List<WorkspaceFile> Files { get; set; } = new();
}

public class TaskExecutionResult
{
    public string Name { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RetryCount { get; set; }
    public long RunDuration { get; set; }

    public static TaskExecutionResult Completed(string name, long elapsed) => new()
    {
        Name = name, Success = true, CompletedAt = DateTime.UtcNow, RunDuration = elapsed,
    };

    public static TaskExecutionResult Failed(string name, Exception ex) => new()
    {
        Name = name, Success = false, Message = ex.Message, CompletedAt = DateTime.UtcNow,
    };
}

public class WorkspaceTaskResult
{
    public static WorkspaceTaskResult Empty() => new();
}

public class WorkspaceFile
{
    public WorkspaceFile()
    {

    }

    public WorkspaceFile(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public string Name { get; set; }
    public string Path { get; set; }
    public string? ContentType { get; set; }
    public string? TaskName { get; set; }
}

public class TaskOutput
{
    public string TaskName { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }
}

public interface IWorkspaceTask<TWorkspace> where TWorkspace : IWorkspace
{
    public string Name { get; }
    Task<WorkspaceTaskResult> ExecuteAsync(ITaskExecutionContext<TWorkspace> context);
}

public interface IWorkspacePipeline
{
    bool CanManage(DiscoveredItem item);
    Task RunAsync(IWorkspace workspace, CancellationToken ct);
}

public interface ITaskExecutionContext<TWorkspace> where TWorkspace : IWorkspace
{
    TWorkspace Workspace { get; set; }
    string TaskName { get; set; }

    WorkspaceFile CreateFile(string name, string path);

    public CancellationToken Canceled { get; set; }
}
