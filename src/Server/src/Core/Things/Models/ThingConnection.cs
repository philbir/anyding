namespace Anyding;

public class ThingConnection : Entity<Guid>
{
    public string Type { get; set; }

    public Thing From { get; set; }

    public Thing To { get; set; }

    public Guid FromId { get; set; }

    public Guid ToId { get; set; }
}
