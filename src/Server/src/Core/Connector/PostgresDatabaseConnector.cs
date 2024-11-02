using Anyding.Connectors;
using Anyding.Data;
using Microsoft.EntityFrameworkCore;

namespace Anyding;

internal class PostgresDatabaseConnector(IAnydingDbContext dbContext) : IConnector
{
    public string Id { get; set; }

    public ValueTask ConnectAsync(ConnectorDefinition definition, CancellationToken ct)
    {
        Id = definition.Id;
        return ValueTask.CompletedTask;
    }

    public Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(DiscoveryFilter filter, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Stream> DownloadAsync(string id, CancellationToken ct)
    {
        DataFile? file = await dbContext.Files.Where(x => x.Id == Guid.Parse(id)).FirstOrDefaultAsync(ct);

        if (file is not null)
        {
            return new MemoryStream(file.Data);
        }

        throw new InvalidOperationException($"File with id '{id}' not found.");
    }

    public async ValueTask<IReadOnlyList<(byte[] data, string id)>> DownloadBatchAsync(IEnumerable<string> ids, CancellationToken ct)
    {
        List<DataFile> files = await dbContext.Files
            .Where(x => ids.Select(Guid.Parse).Contains(x.Id))
            .ToListAsync(ct);

        return files.Select(x => (x.Data, x.Id.ToString("N"))).ToList();
    }

    public async Task<UploadResult> UploadAsync(string id, string path, Stream data, CancellationToken ct)
    {
        var file = new DataFile
        {
            Id = Guid.NewGuid(),
            Name = id,
            Path = path,
            CreateAt = DateTime.UtcNow,
            Data = data.ToByteArray()
        };

        dbContext.Files.Add(file);
        await dbContext.SaveChangesAsync(ct);

        return new UploadResult(file.Id.ToString("N"), file.Data.Length);
    }

    public async ValueTask DeleteAsync(string id, CancellationToken ct)
    {
        await dbContext.Files.Where(x => x.Id == Guid.Parse(id)).ExecuteDeleteAsync(ct);
    }

    public async ValueTask MoveAsync(string id, string path, CancellationToken ct)
    {
        await dbContext.Files.Where(x => x.Id == Guid.Parse(id)).ExecuteUpdateAsync(upd =>
            upd.SetProperty(x => x.Path, path),
            ct);
    }
}

public class DataFile : Entity<Guid>
{
    public string Name { get; set; }

    public string Path { get; set; }

    public DateTime CreateAt { get; set; }
    public byte[] Data { get; set; }
}
