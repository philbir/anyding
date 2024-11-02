namespace Anyding;

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
