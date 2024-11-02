using Anyding.Geo;

namespace Anyding.Search;

public class IndexThingsRequest
{
    public List<IThing> Things { get; set; }
}

public interface IThingsIndexingService
{
    Task IndexAsync(IndexThingsRequest request, CancellationToken ct);
}

public class ThingsIndexingService(IThingsDataDownloader dataDownloader, ISearchDbContext searchDb) : IThingsIndexingService
{
    public async Task IndexAsync(IndexThingsRequest request, CancellationToken ct)
    {
        var context = new IndexingContext();

        foreach (IGrouping<ThingType, IThing> group in request.Things
                     .GroupBy(x => x.Type)
                     .OrderBy(x => x.Key, new ThingTypeForIndexingComparer()))
        {
            switch (group.Key)
            {
                case ThingType.Person:
                    foreach (PersonThing person in group.OfType<PersonThing>())
                    {
                        IndexPerson(context, person);
                    }
                    break;
                case ThingType.Media:
                    foreach (MediaThing media in group.OfType<MediaThing>())
                    {
                        await IndexMediaAsync(context, media, ct);
                    }
                    break;
            }
        }

        searchDb.Persons.CreateDocuments(context.Persons.Values);
        searchDb.Media.CreateDocuments(context.Media.Values);
        searchDb.Faces.CreateDocuments(context.Faces.Values);
    }


    private async Task IndexMediaAsync(
        IndexingContext context,
        MediaThing thing,
        CancellationToken ct)
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

        media.Image = await AddImage(thing, ct);
        context.AddMedia(media);
    }

    private async Task<byte[]> AddImage(MediaThing thing, CancellationToken ct)
    {
        ThingDataReference? originalImage = thing.Data.FirstOrDefault(x => x.Type == "Original");

        if (originalImage == null) return null;

        Stream imageData = await dataDownloader.DownloadAsync(originalImage, ct);
        return imageData.ToByteArray();
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

    private void IndexPersons(IndexingContext context, IEnumerable<PersonThing> persons)
    {
        foreach (PersonThing person in persons)
        {
            IndexPerson(context, person);
        }
    }

    private void IndexPerson(
        IndexingContext context,
        PersonThing thing)
    {
        var personIndex = new PersonIndex
        {
            Id = thing.Id.ToId(), Name = thing.Name, DateOfBirth = thing.Details.DateOfBirth.ToUnixTimeSeconds()
        };

        context.AddPerson(personIndex);
    }
}

public class ThingTypeForIndexingComparer : IComparer<ThingType>
{
    private static readonly Dictionary<ThingType, int> _typeOrder = new()
    {
        { ThingType.Person, 1 }, { ThingType.Media, 2 }, { ThingType.Face, 3 }
    };

    public int Compare(ThingType x, ThingType y)
    {
        if (x == y) return 0;
        if (_typeOrder.TryGetValue(x, out var xOrder) && _typeOrder.TryGetValue(y, out var yOrder))
        {
            return xOrder.CompareTo(yOrder);
        }

        return x.CompareTo(y);
    }
}
