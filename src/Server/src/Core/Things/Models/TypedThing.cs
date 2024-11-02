using Anyding.Media;

namespace Anyding;

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
    public List<ThingDataReference> Data { get => _thing.Data; set => _thing.Data = value; }

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
