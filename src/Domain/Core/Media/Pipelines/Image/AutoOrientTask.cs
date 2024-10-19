using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Anyding.Media.Pipelines;

public class AutoOrientTask : IWorkspaceTask<ImageWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<ImageWorkspace> context)
    {
        Image image = await context.Workspace.LoadWorkingImageAsync(context.Canceled);
        Image oriented = image.Clone(x => x.AutoOrient());

        await oriented.SaveAsync(context.Workspace.WorkingPath, context.Canceled);

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Image.AutoOrient";
    }

}
