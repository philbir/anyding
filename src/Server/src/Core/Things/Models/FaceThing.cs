using Anyding.Media;

namespace Anyding;

public class FaceThing : TypedThing<FaceDetails>
{
    private PersonThing _person;

    public FaceThing(Thing thing) : base(thing)
    {
    }

    public static implicit operator FaceThing(Thing thing)
    {
        return new FaceThing(thing);
    }

    public PersonThing Person
    {
        get
        {
            if (_person is null)
            {
                if (_thing.Connections is not null)
                {
                    ThingConnection? personConnection = _thing.Connections
                        .FirstOrDefault(x => x.Type == ThingConnectionTypes.Face.IsPerson);
                    if (personConnection is not null)
                    {
                        _person = (PersonThing)personConnection.To;
                    }
                }
            }
            else
            {
                return _person;
            }

            return null;
        }
    }
}
