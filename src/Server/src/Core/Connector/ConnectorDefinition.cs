using Anyding.Data;

namespace Anyding.Connectors;

public class ConnectorDefinition : Entity<string>
{
    public string Type { get; set; }
    public string Name { get; set; }
    public IDictionary<string, string> Properties { get; set; }

    public Guid? CredentialId { get; set; }
    public string? Root { get; set; }
    public List<ConnectorMapping> Mapping { get; set; } = [];
    public int Priority { get; set; }
}

public class ConnectorMapping
{
    public string Property { get; set; }

    public string Match { get; set; }
}
