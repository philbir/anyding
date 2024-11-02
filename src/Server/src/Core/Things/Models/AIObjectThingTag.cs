using Anyding.Media;

namespace Anyding;

public class AIObjectThingTag(ThingTag tag) : TypedThingTag<ImageAIObjectTagDetails>(tag)
{
    public static implicit operator AIObjectThingTag(ThingTag thing)
    {
        return new AIObjectThingTag(thing);
    }
}
