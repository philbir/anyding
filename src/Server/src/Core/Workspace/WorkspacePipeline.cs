using System.Diagnostics;
using Anyding.Connectors;
using Anyding.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Anyding;

public abstract class WorkspacePipeline<TWorkspace>(
    IServiceProvider provider)
    : IWorkspacePipeline
    where TWorkspace : IWorkspace
{
    readonly IEnumerable<IWorkspaceTask<TWorkspace>> _tasks = provider.GetServices<IWorkspaceTask<TWorkspace>>();
    readonly ILogger<WorkspacePipeline<TWorkspace>> _logger = provider.GetRequiredService<ILogger<WorkspacePipeline<TWorkspace>>>();
    readonly IMediator _mediator = provider.GetRequiredService<IMediator>();

    protected List<string> _taskNames;
    public abstract string[] ManagedItemTypes { get; }

    public virtual bool CanManage(DiscoveredItem item)
    {
        return ManagedItemTypes.Contains(item.ItemType);
    }

    public virtual async Task RunAsync(IWorkspace workspace, CancellationToken ct)
    {
        _logger.LogInformation("Running pipeline for workspace {WorkspaceId}", workspace.Id);
        workspace.Lock();

        WorkspaceFile? workingFile = workspace.Info.Files.FirstOrDefault(x => x.Name == "Working");

        if (workingFile is null)
        {
            await workspace.CreateWorkingCopyAsync(ct);
        }

        /*
        if (workspace.Info.PipelineResult.All(x => x.Success))
        {
            workspace.UnLock();
            return;
        }*/

        foreach (var tasknames in _taskNames)
        {
            var hasExecuted = workspace.Info.PipelineResult.Any(x => x.Name == tasknames && x.Success);
            if (hasExecuted)
            {
                continue;
            }

            var task = _tasks.FirstOrDefault(x => x.Name == tasknames);
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
                await _mediator.Publish(new WorkspacePipelineTaskCompletedEvent(workspace.Id, task.Name), ct);

                workspace.AddPipelineResult(TaskExecutionResult.Completed(task.Name, stopwatch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Task {TaskName} failed to execute", task.Name);
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
                await _mediator.Publish(new WorkspacePipelineTaskFailedEvent(workspace.Id, task.Name, error.Message), ct);
                throw new ApplicationException($"Task failed to execute {task.Name}", error);
            }
        }

        await _mediator.Publish(new WorkspacePipelineCompletedEvent(workspace.Id), ct);
    }

    protected abstract Task<TWorkspace> InitializeAsync(Guid id, CancellationToken cancellationToken);
}




