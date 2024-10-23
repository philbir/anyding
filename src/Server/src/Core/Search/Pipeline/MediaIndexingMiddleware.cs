using Anyding.Discovery;
using Anyding.Geo;
using Microsoft.Extensions.Logging;

namespace Anyding.Search;

public class MediaIndexingMiddleware : IIndexingMiddleware
{
    private readonly IConnectorFactory _connectorFactory;
    private readonly ILogger<MediaIndexingMiddleware> _logger;
    private IConnector _magicConnector = null;

    public MediaIndexingMiddleware(IConnectorFactory connectorFactory, ILogger<MediaIndexingMiddleware> logger)
    {
        _connectorFactory = connectorFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(IndexingContext context, Func<Task> next)
    {
        await CreateConnectorsAsync();

        var toIndex = context.Request.Things.Where(x => x is MediaThing).ToList();
        var total = toIndex.Count;
        var completed = 0;

        foreach (IThing[] chunk in toIndex.Chunk(250))
        {
            foreach (MediaThing thing in chunk)
            {
                try
                {
                    _logger.LogInformation("{Completed} of {Total} - Indexing Media: {Id}", completed, total, thing.Id);
                    IndexMediaAsync(context, thing);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to index media: {Id}");
                }
                finally
                {
                    completed++;
                }
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
        var media = new MediaIndex { Id = thing.Id.ToId(), Name = thing.Name, Type = Enum.GetName(MediaType.Image) };

        if (thing.Details.GeoLocation is { Latitude: > 0, Longitude: > 0 } loc)
        {
            media.Location = [loc.Latitude.Value, loc.Longitude.Value];
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
            AddGeoNames(geo, media);
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

    private void AddGeoNames(GeoCoding geo, MediaIndex media)
    {
        if (geo.City != null)
        {
            media.GeoNames.Add(new GeoNameIndex("city", geo.City));
        }

        if (geo.Country != null)
        {
            media.GeoNames.Add(new GeoNameIndex("country", geo.Country));
        }

        if (geo.State != null)
        {
            media.GeoNames.Add(new GeoNameIndex("state", geo.State));
        }

        if (geo.County != null)
        {
            media.GeoNames.Add(new GeoNameIndex("county", geo.County));
        }

        if (geo.District != null)
        {
            media.GeoNames.Add(new GeoNameIndex("district", geo.District));
        }

        if (geo.Locality != null)
        {
            media.GeoNames.Add(new GeoNameIndex("locality", geo.Locality));
        }

        if (geo.Label != null)
        {
            media.GeoNames.Add(new GeoNameIndex("label", geo.Label));
        }

        if (geo.PostCode != null)
        {
            media.GeoNames.Add(new GeoNameIndex("postcode", geo.PostCode));
        }

        if (geo.Type != null)
        {
            media.GeoNames.Add(new GeoNameIndex("type", geo.Type));
        }
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
