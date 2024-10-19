namespace Anyding.Media.Pipelines;

public class ExtractVideoDataTask(IVideoProcessingService videoProcessingService)
    : IWorkspaceTask<VideoWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<VideoWorkspace> context)
    {
        var videoFile = context.Workspace.OriginalPath;
        ExtractVideoDataResult videoDataResult = await videoProcessingService
            .ExtractVideoDataAsync(videoFile, context.Canceled);

        await context.Workspace.SaveAsJson(Info.Outputs.Metadata, videoDataResult.Metadata, context.Canceled);
        await context.Workspace.SaveFileAsync(Info.Outputs.Image, videoDataResult.Image, context.Canceled);

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Video.ExtractData";
        public static class Outputs
        {
            public static WorkspaceFile Metadata => new("Metadata", "metadata.json") { TaskName = Name };
            public static WorkspaceFile Image => new ("Image", "image.png") { TaskName = Name };
        }
    }
}
