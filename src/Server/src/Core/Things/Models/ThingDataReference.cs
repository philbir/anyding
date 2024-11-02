namespace Anyding;

public class ThingDataReference : Entity<Guid>
{
    public Guid ThingId { get; set; }
    public string ConnectorId { get; set; }
    public string Identifier { get; set; }
    public string Type { get; set; }
    public string ContentType { get; set; }
    public long? Size { get; set; }
    public string Name { get; set; }
}
