using Anyding.Media;

namespace Anyding;

public class AIImageThingTag(ThingTag tag) : TypedThingTag<ImageAITagDetails>(tag)
{
    public static implicit operator AIImageThingTag(ThingTag thing)
    {
        return new AIImageThingTag(thing);
    }
}
