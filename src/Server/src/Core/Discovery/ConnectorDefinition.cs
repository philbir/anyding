namespace Anyding.Discovery;

public class ConnectorDefinition
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public IDictionary<string, string> Properties { get; set; }

    public Guid? CredentialId { get; set; }
}
