using Anyding.Media;

namespace Anyding;

public class AICaptionThingTag(ThingTag tag) : TypedThingTag<ImageAICaptionTagDetails>(tag)
{
    public static implicit operator AICaptionThingTag(ThingTag thing)
    {
        return new AICaptionThingTag(thing);
    }
}
