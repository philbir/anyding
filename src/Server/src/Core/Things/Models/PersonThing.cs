namespace Anyding;

public class PersonThing(Thing thing) : TypedThing<PersonDetails>(thing)
{
    public static implicit operator PersonThing(Thing thing)
    {
        return new PersonThing(thing);
    }
}
