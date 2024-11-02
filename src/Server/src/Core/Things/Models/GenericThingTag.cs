namespace Anyding;

public class GenericThingTag(ThingTag tag) : TypedThingTag<object>(tag)
{
    public static implicit operator GenericThingTag(ThingTag thing)
    {
        return new GenericThingTag(thing);
    }
}
