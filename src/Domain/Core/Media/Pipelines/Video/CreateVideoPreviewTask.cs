namespace Anyding.Media.Pipelines;

public class CreateVideoPreviewTask(IVideoProcessingService videoProcessingService)
    : IWorkspaceTask<VideoWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<VideoWorkspace> context)
    {
        var outfile = Path.Combine(context.Workspace.RootDirectory, Info.Outputs.Preview720.Path);

        var videoDataResult = await videoProcessingService
            .ConvertTo720Async(context.Workspace.OriginalPath, outfile, context.Canceled);

        context.Workspace.AddFiles(Info.Outputs.Preview720);

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Video.CreatePreview";

        public static class Outputs
        {
            public static WorkspaceFile Preview720 => new("Preview720p", "preview_720p.mp4") { TaskName = Name };
        }
    }
}
