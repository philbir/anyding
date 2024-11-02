using Anyding.Media;

namespace Anyding;

public class AIColorThingTag(ThingTag tag) : TypedThingTag<ImageAIColorTagDetails>(tag)
{
    public static implicit operator AIColorThingTag(ThingTag thing)
    {
        return new AIColorThingTag(thing);
    }
}
