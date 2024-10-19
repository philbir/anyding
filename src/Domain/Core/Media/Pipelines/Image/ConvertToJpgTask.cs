using ImageMagick;

namespace Anyding.Media.Pipelines;

public class ConvertToJpgTask : IWorkspaceTask<ImageWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<ImageWorkspace> context)
    {
        if (Path.GetExtension(
                context.Workspace.WorkingPath).Equals(".jpg", StringComparison.InvariantCultureIgnoreCase))
        {
            return WorkspaceTaskResult.Empty();
        }

        using var image = new MagickImage(context.Workspace.WorkingPath);
        context.Workspace.SetWorkingPath(Path.ChangeExtension(context.Workspace.WorkingPath, ".jpg"));

        await image.WriteAsync(context.Workspace.WorkingPath, context.Canceled);

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Image.ConvertToJpg";
    }
}
