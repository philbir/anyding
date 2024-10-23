using Anyding.Geo;

namespace Anyding.Media.Pipelines;

public class ReverseGeoCodeVideoTask(IGeoDecoderService geoDecoderService) : IWorkspaceTask<VideoWorkspace>
{
    public string Name => Info.Name;

    public async Task<WorkspaceTaskResult> ExecuteAsync(
        ITaskExecutionContext<VideoWorkspace> context)
    {
        VideoMetadata metadata =
            context.Workspace.LoadFromJson<VideoMetadata>(ExtractVideoDataTask.Info.Outputs.Metadata);

        if (metadata.GeoLocation is { Latitude: > 0, Longitude: > 0 } location)
        {
            GeoCoding? place = await geoDecoderService.ReverseAsync(
                location.Latitude.Value,
                location.Longitude.Value,
                context.Canceled);

            if (place != null)
            {
                context.Workspace.SaveAsJson(Info.Outputs.GeoCoding, place, context.Canceled);
            }
        }

        return WorkspaceTaskResult.Empty();
    }

    internal class Info
    {
        public static string Name => "Video.ReverseGeoCode";
        public static class Outputs
        {
            public static WorkspaceFile GeoCoding => new("GeoCoding", "geo_coding.json") { TaskName = Name };
        }
    }
}
