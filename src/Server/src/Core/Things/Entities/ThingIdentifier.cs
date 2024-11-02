namespace Anyding;

public class ThingIdentifier : Entity<Guid>
{
    public Guid ThingId { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}
