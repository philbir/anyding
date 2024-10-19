namespace Anyding.Media.Pipelines;

public class CropFaceImagesTask(IImageCropService imageCropService) : IWorkspaceTask<ImageWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<ImageWorkspace> context)
    {
        await using Stream stream = context.Workspace.LoadWorkingImageStream();

        var faces = context.Workspace.LoadFromJson<IEnumerable<FaceDetectionResult>>(DetectFacesTask.Info.Outputs.DetectedFaces);

        IEnumerable<ImageBoxCropInput> inputs = faces.Select(x => new ImageBoxCropInput
        {
            Box = x.Box,
            Id = x.Id
        });

        IEnumerable<ImageBoxCropResult> faceImages = await imageCropService.CropBoxAsync(
            stream,
            inputs,
            ImageBoxCropOptions.Face,
            context.Canceled);

        foreach (ImageBoxCropResult faceImage in faceImages)
        {
            WorkspaceFile previewFile = context.CreateFile(
                $"Face_{faceImage.Id}",
                $"face_{faceImage.Id}.{faceImage.Info.Format.ToLower()}");

            await context.Workspace.SaveFileAsync(previewFile, faceImage.Image, context.Canceled);
        }
        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Image.CropFaceImages";
    }
}
