using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Anyding;

public class WorkspaceRunner(
    IWorkspaceFactory workspaceFactory,
    IEnumerable<IWorkspacePipeline> pipelines,
    ILogger<WorkspaceRunner> logger)
{
    public async Task RunAsync(CancellationToken ct)
    {
        foreach (var directory in Directory.GetDirectories(workspaceFactory.RootDirectory))
        {
            var id = Guid.Parse(new DirectoryInfo(directory).Name);
            if (Workspace.IsLocked(directory))
            {
                logger.LogInformation($"Workspace is locked: {id}");
                //continue;
            }

            IWorkspace workspace;

            try
            {
                workspace = await workspaceFactory.GetWorkspaceAsync(id, ct);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting workspace {id}: {e.Message}");
                continue;
            }

            IWorkspacePipeline? pipeline = pipelines.FirstOrDefault(p => p.CanManage(workspace.Info.Discovery));

            if (pipeline is null)
            {
                logger.LogWarning($"No pipeline found for {workspace.Info.Discovery.ItemType}");
                continue;
            }

            try
            {
                AsyncRetryPolicy? retryPolicy = Policy
                    .Handle<IOException>()
                    .WaitAndRetryAsync(3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            Console.WriteLine(
                                $"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                        });

                retryPolicy.ExecuteAsync(async () => await pipeline.RunAsync(workspace, ct));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error running pipeline for {Id}", id);
            }
        }
    }
}
