using System.Diagnostics;
using Anyding.Discovery;

namespace Anyding;

public abstract class WorkspacePipeline<TWorkspace>(
    IEnumerable<IWorkspaceTask<TWorkspace>> tasks)
    : IWorkspacePipeline
    where TWorkspace : IWorkspace
{
    protected List<string> _taskNames;
    public abstract string[] ManagedItemTypes { get; }

    public virtual bool CanManage(DiscoveredItem item)
    {
        return ManagedItemTypes.Contains(item.ItemType);
    }

    public virtual async Task RunAsync(IWorkspace workspace, CancellationToken ct)
    {
        workspace.Lock();

        WorkspaceFile? workingFile = workspace.Info.Files.FirstOrDefault(x => x.Name == "Working");

        if (workingFile is null)
        {
            await workspace.CreateWorkingCopyAsync(ct);
        }

        //All done
        if (workspace.Info.PipelineResult.All(x => x.Success))
        {
            workspace.UnLock();
            return;
        }

        foreach (var tasknames in _taskNames)
        {
            var hasExecuted = workspace.Info.PipelineResult.Any(x => x.Name == tasknames && x.Success);
            if (hasExecuted)
            {
                continue;
            }

            var task = tasks.FirstOrDefault(x => x.Name == tasknames);
            Exception error = null;

            try
            {
                var stopwatch = Stopwatch.StartNew();

                var executionContext = new TaskExecutionContext<TWorkspace>
                {
                    Workspace =  (TWorkspace) workspace,
                    TaskName = tasknames,
                    Canceled = ct,
                };

                WorkspaceTaskResult result = await task.ExecuteAsync(executionContext);
                workspace.AddPipelineResult(TaskExecutionResult.Completed(task.Name, stopwatch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                workspace.AddPipelineResult(TaskExecutionResult.Failed(task.Name, ex));
                error = ex;
            }
            finally
            {
                await workspace.SaveInfoAsync(ct);
                workspace.UnLock();
            }

            if (error != null)
            {
                throw new ApplicationException($"Task failed to execute {task.Name}", error);
            }
        }
    }

    protected abstract Task<TWorkspace> InitializeAsync(Guid id, CancellationToken cancellationToken);
}




