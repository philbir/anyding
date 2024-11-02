using System.Security.Cryptography;
using System.Text;
using Anyding.Connector;
using Anyding.Connectors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Anyding;

public interface IConnectorFactory
{
    Task<IConnector> CreateConnectorAsync(
        string? id,
        CancellationToken ct);

    Task<IConnector> CreateConnectorAsync(
        Dictionary<string, string> matchProperties,
        CancellationToken ct);
}

public class DefaultConnectorFactory(
    IEnumerable<IConnectorManager> connectionManagers,
    IConnectorDefinitionQuery connectorDefinitionQuery,
    IMemoryCache memoryCache,
    IOptions<ConnectorOptions> options
) : IConnectorFactory
{
    public async Task<IConnector> CreateConnectorAsync(
        string? id,
        CancellationToken ct)
    {
        ConnectorDefinition definition = await connectorDefinitionQuery.ByIdAsync(
            id ?? options.Value.Default, ct);

        IConnectorManager? manager = connectionManagers.SingleOrDefault(
            x => x.ManagedTypes.Contains(definition.Type));

        if (manager is null)
        {
            throw new InvalidOperationException(
                $"No manager found for type: {definition.Type}");
        }

        return await manager.CreateAsync(definition, ct);
    }

    public async Task<IConnector> CreateConnectorAsync(Dictionary<string, string> matchProperties, CancellationToken ct)
    {
        var cacheKey = GenerateCacheKey(matchProperties);

        if (memoryCache.TryGetValue(cacheKey, out IConnector cachedConnector))
        {
            return cachedConnector;
        }

        IReadOnlyList<ConnectorDefinition> allDefinitions = await connectorDefinitionQuery.AllAsync(ct);

        foreach (ConnectorDefinition definition in allDefinitions.OrderBy(x => x.Priority))
        {
            foreach (ConnectorMapping map in definition.Mapping)
            {
                if (matchProperties.TryGetValue(map.Property, out var value) &&
                    map.Match.Equals(value, StringComparison.OrdinalIgnoreCase))
                {

                    IConnector connector = await CreateConnectorAsync(definition.Id, ct);
                    memoryCache.Set(cacheKey, connector);
                    return connector;
                }
            }
        }

        return await CreateConnectorAsync(id: null, ct);;
    }

    private string GenerateCacheKey(Dictionary<string, string> matchProperties)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashInput = string.Join(";", matchProperties.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
