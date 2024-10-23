using SixLabors.ImageSharp;

namespace Anyding.Media.Pipelines;

public class CreatePreviewImagesFromVideoTask(IImagePreviewService imagePreviewService) : IWorkspaceTask<VideoWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<VideoWorkspace> context)
    {
        Stream imageStream = context.Workspace.LoadFileStream(ExtractVideoDataTask.Info.Outputs.Image);

        using Image image = await Image.LoadAsync(imageStream, context.Canceled);

        foreach (PreviewImageSizeDefinition definition in PreviewImageSizeDefinition.Defaults)
        {
            CreatePreviewImageResult result = await imagePreviewService.CreatePreviewAsync(
                image,
                definition.Name,
                context.Canceled);

            WorkspaceFile previewFile = context.CreateFile(
                $"Preview_{definition.Name}",
                $"preview_{definition.Name}_{result.Info.Dimension.Width}x{result.Info.Dimension.Height}.webp");

            await context.Workspace.SaveFileAsync(previewFile, result.Preview, context.Canceled);
        }

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Video.CreatePreviewImages";
    }
}
