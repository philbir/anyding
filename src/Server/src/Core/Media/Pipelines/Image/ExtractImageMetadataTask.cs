

namespace Anyding.Media.Pipelines;

public class ExtractImageMetadataTask(IImageMetadataExtractor metadataExtractor) : IWorkspaceTask<ImageWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<ImageWorkspace> context)
    {
        await using Stream stream = context.Workspace.LoadWorkingImageStream();
        ImageMetadata metadata = await metadataExtractor.GetMetadataAsync(stream, context.Canceled);

        context.Workspace.SaveAsJson(Info.Outputs.Metadata, metadata, context.Canceled);

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Image.ExtractMetadata";
        public static class Outputs
        {
            public static WorkspaceFile Metadata => new("Metadata", "metadata.json") { TaskName = Name };
        }
    }
}
