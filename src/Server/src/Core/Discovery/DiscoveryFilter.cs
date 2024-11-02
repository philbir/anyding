namespace Anyding.Connectors;

public class DiscoveryFilter
{
    public string Path { get; set; }

    public string Filter { get; set; } = "*";

    public bool IncludeChildren { get; set; }

    public int MaxItems { get; set; } = 1000;
}
