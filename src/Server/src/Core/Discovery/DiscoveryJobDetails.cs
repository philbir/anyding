using System.Security.Cryptography;
using Anyding.Jobs;

namespace Anyding.Connectors;

public class DiscoveryJobDetails : IJobDetails
{
    public string ConnectorId { get; set; }

    public string ItemType { get; set; }

    public DiscoveryFilter Filter { get; set; }
}
