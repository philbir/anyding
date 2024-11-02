namespace Anyding.Connectors;

public class DiscoveredItem
{
    public string Id { get; set; }
    public string ConnectorId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ItemType { get; set; }
}
