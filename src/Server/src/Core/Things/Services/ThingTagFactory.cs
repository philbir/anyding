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
