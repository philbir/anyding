using Anyding.Geo;
using SixLabors.ImageSharp;

namespace Anyding.Media.Pipelines;

public class ImagePipeline : WorkspacePipeline<ImageWorkspace>
{
    public ImagePipeline(
        IServiceProvider provider) : base(provider)
    {
        _taskNames =
        [
            ConvertToJpgTask.Info.Name,
            AutoOrientTask.Info.Name,
            ExtractImageMetadataTask.Info.Name,
            ReverseGeoCodeImageTask.Info.Name,
            CreatePreviewImagesTask.Info.Name,
            DetectFacesTask.Info.Name,
            CropFaceImagesTask.Info.Name
        ];
    }

    public override string[] ManagedItemTypes => [ItemTypeNames.Image];

    protected override async Task<ImageWorkspace> InitializeAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}

public class ImageWorkspace(Guid id, string directory) : Workspace(id, directory)
{
    internal async Task<Image> LoadWorkingImageAsync(CancellationToken cancellationToken)
    {
        return await Image.LoadAsync(WorkingPath, cancellationToken);
    }

    internal Stream LoadWorkingImageStream()
    {
        return File.OpenRead(WorkingPath);
    }

    public ImageMetadata? GetMetadata()
    {
        return LoadFromJson<ImageMetadata>(ExtractImageMetadataTask.Info.Outputs.Metadata);
    }

    internal GeoCoding? GetGeoCoding()
    {
        return LoadFromJson<GeoCoding>(ReverseGeoCodeImageTask.Info.Outputs.GeoCoding);
    }

    public IReadOnlyList<WorkspaceFile> GetPreviews()
    {
        return Info.Files
            .Where(f => f.TaskName is not null && f.TaskName == CreatePreviewImagesTask.Info.Name)
            .ToList();
    }

    public IReadOnlyList<FaceDetectionResult> GetDetectedFaces()
    {
        return LoadFromJson< List<FaceDetectionResult>>(DetectFacesTask.Info.Outputs.DetectedFaces);
    }

    public WorkspaceFile? GetFaceImage(Guid faceId)
    {
        var name = CropFaceImagesTask.Info.GetFaceImageName(faceId);
        return Info.Files.FirstOrDefault(f => f.Name == name);
    }
}

public class ImageWorkspaceProvider : IWorkspaceProvider
{
    public IWorkspace CreateWorkspace(Guid id, string directory)
    {
        return new ImageWorkspace(id, directory);
    }

    public string[] ManagedItemTypes => [ItemTypeNames.Image];
}
