namespace Anyding;

public class TaskExecutionContext<TWorkspace> : ITaskExecutionContext<TWorkspace> where TWorkspace : IWorkspace
{
    public TWorkspace Workspace { get; set; }
    public string TaskName { get; set; }
    public CancellationToken Canceled { get; set; }

    public WorkspaceFile CreateFile(string name, string path)
    {
        return new WorkspaceFile
        {
            Name = name,
            Path = path,
            TaskName = TaskName,
        };
    }
}
