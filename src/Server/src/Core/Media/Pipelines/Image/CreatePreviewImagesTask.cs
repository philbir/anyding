using Image = SixLabors.ImageSharp.Image;

namespace Anyding.Media.Pipelines;

public class CreatePreviewImagesTask(IImagePreviewService imagePreviewService) : IWorkspaceTask<ImageWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<ImageWorkspace> context)
    {
        using Image image = await context.Workspace.LoadWorkingImageAsync(context.Canceled);

        foreach (PreviewImageSizeDefinition definition in PreviewImageSizeDefinition.Defaults)
        {
            CreatePreviewImageResult result = await imagePreviewService.CreatePreviewAsync(
                image, definition.Name,
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
        public static string Name => "Image.CreatePreview";
    }
}
