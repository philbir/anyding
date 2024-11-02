using Anyding.Geo;
using Anyding.Media;
using Anyding.Media.Pipelines;

namespace Anyding;


public class ImageWorkspaceHarvester
    : IThingsWorkspaceHarvester
{
    public string[] MangedItemTypes => [ItemTypeNames.Image];

    public async Task<CreateThingsRequest> ExecuteAsync(IWorkspace workspace, CancellationToken ct)
    {
        try
        {
            if (workspace is ImageWorkspace imageWorkspace)
            {
                CreateThingsRequest request = await CreateThingsFromImageAsync(imageWorkspace, ct);
                return request;
            }

            throw new Exception("Workspace is not an image workspace");
        }
        catch (Exception e)
        {
            throw new Exception("Error while harvesting image workspace", e);
        }
    }

    private async Task<CreateThingsRequest> CreateThingsFromImageAsync(
        ImageWorkspace workspace,
        CancellationToken ct)
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
                LoadData = workspace.GetFileLoader(preview),
                Type = "Preview"
            });
        }

        createRequest.Things.Add(imageThing);
        // Faces
        AddFaces(workspace, createRequest, detectedFaces, imageThing);

        return createRequest;
    }

    private static void AddFaces(
        ImageWorkspace workspace,
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
            WorkspaceFile? faceImageFile = workspace.GetFaceImage(face.Id);

            if (faceImageFile != null)
            {
                faceThing.Data.Add(new ThingInputData
                {
                    Id = faceImageFile.Path,
                    Name = faceImageFile.Name,
                    Type = "FaceImage",
                    ContentType = faceImageFile.ContentType,
                    LoadData = workspace.GetFileLoader(faceImageFile)
                });
            }

            createRequest.Things.Add(faceThing);
            createRequest.Connections.Add(new()
            {
                Id = Guid.NewGuid(), Type = ThingConnectionTypes.Face.InMedia, FromId = imageThing.Id, ToId = faceThing.Id
            });
        }
    }


}
