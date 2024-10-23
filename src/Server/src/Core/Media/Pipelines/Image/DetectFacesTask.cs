namespace Anyding.Media.Pipelines;

public class DetectFacesTask(IFaceDetectionService faceDetectionService) : IWorkspaceTask<ImageWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<ImageWorkspace> context)
    {
        await using Stream stream = context.Workspace.LoadWorkingImageStream();

        IEnumerable<FaceDetectionResult> faces = await faceDetectionService.DetectFacesAsync(stream, context.Canceled);

        context.Workspace.SaveAsJson(Info.Outputs.DetectedFaces, faces, context.Canceled);

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Image.DetectFaces";

        public static class Outputs
        {
            public static WorkspaceFile DetectedFaces => new("DetectedFaces", "detected_faces.json") { TaskName = Name };
        }
    }
}
