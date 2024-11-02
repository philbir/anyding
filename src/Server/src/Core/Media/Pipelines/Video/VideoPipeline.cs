namespace Anyding.Media.Pipelines;

public class VideoPipeline : WorkspacePipeline<VideoWorkspace>
{
    public VideoPipeline(
        IServiceProvider provider) : base(provider)
    {
        _taskNames = [
            ExtractVideoDataTask.Info.Name,
            ReverseGeoCodeVideoTask.Info.Name,
            CreateVideoPreviewTask.Info.Name,
            CreatePreviewImagesFromVideoTask.Info.Name,
        ];
    }

    public override string[] ManagedItemTypes => ["MOV", "MP4"];

    protected override async Task<VideoWorkspace> InitializeAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class VideoWorkspace(Guid id, string directory) : Workspace(id, directory);

public class VideoWorkspaceProvider : IWorkspaceProvider
{
    public IWorkspace CreateWorkspace(Guid id, string directory)
    {
        return new ImageWorkspace(id, directory);
    }

    public string[] ManagedItemTypes => [ItemTypeNames.Video];
}
