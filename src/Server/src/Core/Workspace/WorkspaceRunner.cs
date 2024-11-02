using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Anyding;

public class WorkspaceRunner(
    IWorkspaceFactory workspaceFactory,
    IEnumerable<IWorkspacePipeline> pipelines,
    ILogger<WorkspaceRunner> logger)
{
    public async Task RunAsync(Guid id, CancellationToken ct)
    {
        var directory = workspaceFactory.GetWorkspaceDirectory(id);

        if (Workspace.IsLocked(directory.FullName))
        {
            logger.LogInformation($"Workspace is locked: {id}");
            return;
        }

        IWorkspace workspace;

        try
        {
            workspace = await workspaceFactory.GetWorkspaceAsync(id, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error getting workspace {id}", id);
            return;
        }

        IWorkspacePipeline? pipeline = pipelines.FirstOrDefault(p => p.CanManage(workspace.Info.Discovery));

        if (pipeline is null)
        {
            logger.LogWarning($"No pipeline found for {workspace.Info.Discovery.ItemType}");
            return;
        }

        try
        {
            AsyncRetryPolicy? retryPolicy = Policy
                .Handle<IOException>()
                .WaitAndRetryAsync(3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            $"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                    });

            await retryPolicy.ExecuteAsync(async () => await pipeline.RunAsync(workspace, ct));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running pipeline for {Id}", id);
        }
    }

    public async Task RunAllAsync(CancellationToken ct)
    {
        foreach (var directory in Directory.GetDirectories(workspaceFactory.RootDirectory))
        {
            var id = Guid.Parse(Path.GetFileName(directory));
            await RunAsync(id, ct);
        }
    }
}
