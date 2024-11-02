using Anyding.Media;

namespace Anyding;

public class MediaThing : TypedThing<MediaDetails>
{
    private List<FaceThing> _faces;

    public MediaThing(Thing thing) : base(thing)
    {
    }

    public List<FaceThing> Faces
    {
        get
        {
            if (_thing.Connections is not null)
            {
                if (_faces is null)
                {
                    _faces = _thing.Connections
                        .Where(x => x.Type == ThingConnectionTypes.Face.InMedia)
                        .Select(x => (FaceThing)x.To)
                        .ToList();
                }

                return _faces;
            }

            return Array.Empty<FaceThing>().ToList();
        }
    }

    public static implicit operator MediaThing(Thing thing)
    {
        return new MediaThing(thing);
    }
}
