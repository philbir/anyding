using Anyding.Media;

namespace Anyding;

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
