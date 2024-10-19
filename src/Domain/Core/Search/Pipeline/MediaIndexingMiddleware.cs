using Anyding.Discovery;

namespace Anyding.Search;

public class MediaIndexingMiddleware : IIndexingMiddleware
{
    private readonly IConnectorFactory _connectorFactory;
    private IConnector _magicConnector = null;

    public MediaIndexingMiddleware(IConnectorFactory connectorFactory)
    {
        _connectorFactory = connectorFactory;
    }

    public async Task InvokeAsync(IndexingContext context, Func<Task> next)
    {
        await CreateConnectorsAsync();

        foreach (IThing[] chunk in context.Request.Things.Where(x => x is MediaThing).Chunk(250))
        {
            foreach (MediaThing thing in chunk)
            {
                IndexMediaAsync(context, thing);
            }
        }

        await next();
    }

    private async Task CreateConnectorsAsync()
    {
        return;

        _magicConnector = await _connectorFactory.CreateConnectorAsync(
            new ConnectorDefinition
            {
                Id = Guid.NewGuid(),
                Name = "MagicMedia",
                Type = "LFS",
                Properties = new Dictionary<string, string>() { ["Root"] = "/Users/p7e/nas/home/Photos/Moments" }
            }, CancellationToken.None);
    }

    private async Task IndexMediaAsync(
        IndexingContext context,
        MediaThing thing)
    {
        var media = new MediaIndex
        {
            Id = thing.Id.ToId(),
            Name = thing.Name,
            Type = Enum.GetName(MediaType.Image)
        };

        if (thing.Details.GeoLocation is { } loc)
        {
            media.Location = [loc.Latitude, loc.Longitude];
            media.Altitude = loc.Altitude;
        }

        media.Dimension = thing.Details.Dimension;
        media.Orientation = thing.Details.Orientation;
        media.DateTaken = thing.Details.DateTaken.ToDateIndex();

        if (thing.Details.GeoCoding is { } geo)
        {
            media.City = geo.City;
            media.Country = geo.Country;
            media.PlaceName = geo.Name;
            media.Street = geo.Street;
            media.CountryCode = geo.CountryCode;
        }

        foreach (FaceThing face in thing.Faces)
        {
            IndexFace(context, thing, face);
            media.Faces?.Add(new FaceInMediaIndex
            {
                FaceId = face.Id.ToId(),
                PersonId = face?.Person?.Id.ToId(),
                PersonName = face.Person?.Name,
                AgeInMonths = face.Details.AgeInMonth
            });
        }

        foreach (IThingTag tag in thing.Tags)
        {
            media.Tags.Add(new ThingTagIndex { Name = tag.Definition.Name, Value = tag.Value });
        }

        if (_magicConnector is not null)
        {
            var working = thing.Data.FirstOrDefault(x => x.Type == "Original");
            if (working is not null)
            {
                Stream imageData = await _magicConnector.DownloadAsync(working.Identifier, context.Cancelled);
                media.Image = imageData.ToByteArray();
            }
        }

        context.AddMedia(media);
    }

    private void IndexFace(
        IndexingContext context,
        MediaThing mediaThing,
        FaceThing faceThing)
    {
        var faceIndex = new FaceIndex
        {
            Id = faceThing.Id.ToId(),
            Encoding = faceThing.Details.Encoding,
            Name = faceThing.Name,
            State = faceThing.Details.State,
            RecognitionType = faceThing.Details.RecognitionType,
            MediaId = mediaThing.Id.ToId(),
            PersonId = faceThing.Person?.Id.ToId()
        };

        if (faceThing.Details.AgeInMonth is { } age)
        {
            faceIndex.AgeInMonth = age;
        }

        context.AddFace(faceIndex);
    }
}
