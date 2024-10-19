using System.Collections;
using Anyding.Media;

namespace Anyding;

public interface IThing
{
    Guid Id { get; set; }

    string Name { get; set; }

    string? Description { get; set; }

    ThingType Type { get; set; }

    ThingClass? Class { get; set; }

    ThingSource? Source { get; set; }

    ThingState Status { get; set; }

    DateTimeOffset? Date { get; set; }

    DateTime? Created { get; set; }

    public List<ThingStorageReference> Data { get;  set; }
}

public class MediaThing : TypedThing<MediaDetails>
{
    private List<FaceThing> _faces;

    public MediaThing(Thing thing) : base(thing)
    {

    }

    public List<FaceThing> Faces
    {
        get
        {
            if (_thing.Connections is not null)
            {
                if (_faces is null)
                {
                    _faces = _thing.Connections
                        .Where(x => x.Type == ThingConnectionTypes.Face.InMedia)
                        .Select(x => (FaceThing)x.To)
                        .ToList();
                }

                return _faces;
            }

            return Array.Empty<FaceThing>().ToList();
        }
    }

    public static implicit operator MediaThing(Thing thing)
    {
        return new MediaThing(thing);
    }
}

public class FaceThing : TypedThing<FaceDetails>
{
    private PersonThing _person;

    public FaceThing(Thing thing) : base(thing)
    {
    }

    public static implicit operator FaceThing(Thing thing)
    {
        return new FaceThing(thing);
    }

    public PersonThing Person
    {
        get
        {
            if (_person is null)
            {
                if (_thing.Connections is not null)
                {
                    ThingConnection? personConnection = _thing.Connections
                        .FirstOrDefault(x => x.Type == ThingConnectionTypes.Face.IsPerson);
                    if (personConnection is not null)
                    {
                        _person = (PersonThing)personConnection.To;
                    }
                }
            }
            else
            {
                return _person;
            }
            
            return null;
        }
    }

}

public abstract class TypedThing<TDetails> : IThing
{
    protected readonly Thing _thing;
    protected TDetails? _details;

    protected TypedThing(Thing thing)
    {
        _thing = thing;
    }

    public Guid Id { get => _thing.Id; set => _thing.Id = value; }
    public string Name { get => _thing.Name; set => _thing.Name = value; }
    public string? Description { get => _thing.Description; set => _thing.Description = value; }
    public ThingType Type { get => _thing.Type; set => _thing.Type = value; }
    public ThingClass? Class { get => _thing.Class; set => _thing.Class = value; }
    public ThingSource? Source { get => _thing.Source; set => _thing.Source = value; }
    public ThingState Status { get => _thing.Status; set => _thing.Status = value; }
    public DateTimeOffset? Date { get => _thing.Date; set => _thing.Date = value; }
    public DateTime? Created { get => _thing.Created; set => _thing.Created = value; }
    public List<ThingStorageReference> Data { get => _thing.Data; set => _thing.Data = value; }

    public List<IThingTag> Tags => CreateTags();

    private List<IThingTag> CreateTags()
    {
        return _thing?.Tags.Select(ThingTagFactory.Create).ToList() ?? [];
    }

    public TDetails Details
    {
        get
        {
            if (_details is null)
            {
                _details = ThingDetailSerializer.Deserialize<TDetails>(_thing.Detail);
            }

            return _details;
        }
        set
        {
            _details = value;
            _thing.Detail = ThingDetailSerializer.SerializeToJson(value);
        }
    }
}

public class PersonThing(Thing thing) : TypedThing<PersonDetails>(thing)
{
    public static implicit operator PersonThing(Thing thing)
    {
        return new PersonThing(thing);
    }
}

public class DeviceThing : TypedThing<CameraDetails>
{
    public DeviceThing(Thing thing) : base(thing)
    {
    }

    public static implicit operator DeviceThing(Thing thing)
    {
        return new DeviceThing(thing);
    }
}

public class Thing
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }
    public string? Detail { get; set; }
    public ThingType Type { get; set; }
    public ThingClass? Class { get; set; }
    public ThingSource? Source { get; set; }
    public ThingState Status { get; set; }

    public DateTimeOffset? Date { get; set; }

    public DateTime? Created { get; set; }
    public List<ThingTag> Tags { get; set; } = new List<ThingTag>();

    //public IReadOnlyList<ThingPreview> Thumbnails { get; set; } = new List<ThingPreview>();
    public List<ThingConnection> Connections { get; set; } = new List<ThingConnection>();
    public List<ThingStorageReference> Data { get; set; } = new List<ThingStorageReference>();

    public List<ThingIdentifier> Identifiers { get; set; } = new List<ThingIdentifier>();

    //public IReadOnlyList<ThingProperty> Properties { get; set; } = new List<ThingProperty>();
}

public enum ThingType
{
    Media = 1,
    Face = 2,
    Person = 3,
    Document = 4,
    Device = 5
}

public static class ThingConnectionTypes
{
    public static class Media
    {
        public const string TakenWith = nameof(TakenWith);
    }

    public static class Face
    {
        public const string IsPerson = nameof(IsPerson);

        public const string InMedia = nameof(InMedia);
    }
}

public interface IThingTag
{
    Guid Id { get; set; }

    TagDefinition Definition { get; set; }

    string? Value { get; set; }
}

public class TypedThingTag<TDetails> : IThingTag
{
    private readonly ThingTag _thingTag;
    private TDetails _details;

    public TypedThingTag(ThingTag thingTag)
    {
        _thingTag = thingTag;
    }

    public Guid Id { get => _thingTag.Id; set => _thingTag.Id = value; }
    public TagDefinition Definition { get => _thingTag.Definition; set => _thingTag.Definition = value; }
    public string? Value { get => _thingTag.Value; set => _thingTag.Value = value; }

    public TDetails Detail
    {
        get
        {
            if (_details is null)
            {
                _details = ThingDetailSerializer.Deserialize<TDetails>(_thingTag.Detail);
            }

            return _details;
        }
        set
        {
            _details = value;
            _thingTag.Detail = ThingDetailSerializer.SerializeToJson(value);
        }
    }
}

public class ThingTag
{
    public Guid Id { get; set; }

    public Guid ThingId { get; set; }

    public TagDefinition Definition { get; set; }

    public Guid DefinitionId { get; set; }
    public string? Value { get; set; }

    public string Detail { get; set; }
}

public class TagDefinition
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Color { get; set; }

    public string? Icon { get; set; }
}

public class ThingConnection
{
    public Guid Id { get; set; }

    public string Type { get; set; }

    public Thing From { get; set; }

    public Thing To { get; set; }

    public Guid FromId { get; set; }

    public Guid ToId { get; set; }
}

public class ThingStorageReference
{
    public Guid Id { get; set; }
    public Guid ThingId { get; set; }
    public Guid ConnectorId { get; set; }

    public string Identifier { get; set; }

    public string Type { get; set; }

    public string ContentType { get; set; }
}

public class ThingIdentifier
{
    public Guid Id { get; set; }

    public Guid ThingId { get; set; }

    public string Type { get; set; }

    public string Value { get; set; }
}

public class ThingPreview : ImageData
{
    public string Id { get; set; }

    public int PageNumber { get; set; }
}

public class ImageInfo
{
    public ImageFormat Format { get; set; }

    public ImageSize Size { get; set; }
}

public class ImageData : ImageInfo
{
    public byte[] Data { get; set; }
}

public enum ImageFormat
{
    WebP,
    Png
}

public class ImageSize
{
    public int Height { get; set; }

    public int Width { get; set; }
}

public enum ThingState
{
    Draft,
    Processing,
    New,
    Active,
    Archived,
    Deleted
}

public class ThingStorage : ThingStorageReference
{
    public Stream Stream { get; set; }
}


public class ThingClass
{
    public Guid Id { get; set; }

    public ThingType Type { get; set; }

    public string Name { get; set; }
}

public static class ThingClassNames
{
    public const string Image = "Image";
    public const string Video = "Video";
    public const string Camera = "Camera";
}


public class ThingSource
{
    public string SourceId { get; set; }

    public Guid ConnectorId { get; set; }

    public string? FullLocation { get; set; }
}


public class PersonDetails : IThingDetail
{
    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public string? Nickname { get; set; }

    public DateOnly? DateOfBirth { get; set; }
}

public class CameraDetails : IThingDetail
{
    public string Make { get; set; }

    public string Model { get; set; }

    public string Title { get; set; }
}
