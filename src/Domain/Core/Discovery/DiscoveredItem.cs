namespace Anyding.Discovery;

public class DiscoveredItem
{
    public string Id { get; set; }
    public Guid ConnectorId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ItemType { get; set; }
}
