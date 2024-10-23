using Anyding.Media;

namespace Anyding;

public class ThingTagFactory
{
    public static IThingTag Create(ThingTag thingTag)
    {
        switch (thingTag.Definition.Name)
        {
            case "AICaption":
                return new AICaptionThingTag(thingTag);
            case "AIObject":
                return new AIObjectThingTag(thingTag);
            case "AIImage":
                return new AIImageThingTag(thingTag);
            case "AIColor":
                return new AIColorThingTag(thingTag);
            default:
                return new GenericThingTag(thingTag);
        }
    }
}

public class GenericThingTag(ThingTag tag) : TypedThingTag<object>(tag)
{
    public static implicit operator GenericThingTag(ThingTag thing)
    {
        return new GenericThingTag(thing);
    }
}

public class AICaptionThingTag(ThingTag tag) : TypedThingTag<ImageAICaptionTagDetails>(tag)
{
    public static implicit operator AICaptionThingTag(ThingTag thing)
    {
        return new AICaptionThingTag(thing);
    }
}

public class AIObjectThingTag(ThingTag tag) : TypedThingTag<ImageAIObjectTagDetails>(tag)
{
    public static implicit operator AIObjectThingTag(ThingTag thing)
    {
        return new AIObjectThingTag(thing);
    }
}

public class AIImageThingTag(ThingTag tag) : TypedThingTag<ImageAITagDetails>(tag)
{
    public static implicit operator AIImageThingTag(ThingTag thing)
    {
        return new AIImageThingTag(thing);
    }
}

public class AIColorThingTag(ThingTag tag) : TypedThingTag<ImageAIColorTagDetails>(tag)
{
    public static implicit operator AIColorThingTag(ThingTag thing)
    {
        return new AIColorThingTag(thing);
    }
}
