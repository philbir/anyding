using Anyding.Geo;
using Anyding.Ingest;
using Anyding.Media;
using Anyding.Media.Pipelines;

namespace Anyding;

public class ImageThingsFactory(IWorkspaceFactory workspaceFactory, ThingIngestionPipeline pipeline)
{
    public async Task<IReadOnlyList<Thing>> CreateFromWorkspaceAsync(Guid id, CancellationToken ct)
    {
        var workspace = (ImageWorkspace)await workspaceFactory.GetWorkspaceAsync(id, ct);

        if (workspace.Info.ItemType == ItemTypeNames.Image)
        {
            try
            {
                CreateThingsRequest request = await CreateThingsFromImageAsync(workspace, ct);
                await pipeline.RunAsync(request, ct);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return null;
    }

    private async Task<CreateThingsRequest> CreateThingsFromImageAsync(ImageWorkspace workspace, CancellationToken ct)
    {
        var metadata = workspace.GetMetadata();
        GeoCoding? geoCoding = workspace.GetGeoCoding();
        var detectedFaces = workspace.GetDetectedFaces();
        var previews = workspace.GetPreviews();

        var createRequest = new CreateThingsRequest();

        var mediaDetail = new MediaDetails
        {
            Filename = Path.GetFileName(workspace.Info.Discovery.Id),
            Path = Path.GetDirectoryName(workspace.Info.Discovery.Id),
            Dimension = metadata.Dimension,
            DateTaken = metadata.DateTaken,
            Orientation = metadata.Orientation,
            //Lens = metadata.Lens,
            GeoLocation = metadata.GeoLocation,
            GeoCoding = geoCoding,
        };

        var imageThing = new CreateThingInput
        {
            Id = Guid.NewGuid(),
            Name = mediaDetail.Filename,
            Created = DateTime.UtcNow,
            Type = ThingType.Media,
            ClassName = ThingClassNames.Image,
            Details = mediaDetail.SerializeToJson(),
            Source = new ThingSource
            {
                ConnectorId = workspace.Info.Discovery.ConnectorId, SourceId = workspace.Info.Discovery.Id
            }
        };
        imageThing.Data.Add(new ThingInputData
        {
            Id = Path.GetFileName(workspace.Info.Discovery.Id),
            Name = "Original",
            Type = "Original",
            ContentType = "image/" + Path.GetExtension(workspace.OriginalPath).TrimStart('.'),
            LoadData = () => File.OpenRead(workspace.OriginalPath)
        });

        imageThing.Data.Add(new ThingInputData
        {
            Id = Path.GetFileName(workspace.WorkingPath),
            Name = "Working",
            Type = "Working",
            ContentType = "image/" + Path.GetExtension(workspace.WorkingPath).TrimStart('.'),
            LoadData = workspace.LoadWorkingImageStream
        });

        foreach (WorkspaceFile preview in previews)
        {
            imageThing.Data.Add(new ThingInputData
            {
                Id = preview.Path,
                Name = preview.Name,
                ContentType = "image/" + Path.GetExtension(preview.Path).TrimStart('.'),
                LoadData = () => File.OpenRead(Path.Combine(workspace.RootDirectory,  preview.Path)),
                Type = "Preview"
            });
        }

        createRequest.Things.Add(imageThing);
        // Faces
        AddFaces(createRequest, detectedFaces, imageThing);

        return createRequest;
    }

    private static void AddFaces(
        CreateThingsRequest createRequest,
        IReadOnlyList<FaceDetectionResult> detectedFaces,
        CreateThingInput imageThing)
    {
        foreach (FaceDetectionResult face in detectedFaces)
        {
            var faceDetails = new FaceDetails { Encoding = face.Encoding, Box = face.Box };

            var faceThing = new CreateThingInput()
            {
                Id = face.Id,
                Name = "Face",
                Created = DateTime.UtcNow,
                Type = ThingType.Face,
                Details = faceDetails.SerializeToJson()
            };

            createRequest.Things.Add(faceThing);
            createRequest.Connections.Add(new()
            {
                Id = Guid.NewGuid(), Type = "FaceInImage", FromId = imageThing.Id, ToId = faceThing.Id
            });
        }
    }
}


