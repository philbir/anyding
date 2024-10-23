using Anyding.Discovery;

namespace Anyding;

internal class FileSystemConnector : IConnector
{
    public Guid Id { get; set; }

    private string Root { get; set; } = "/";

    public ValueTask ConnectAsync(ConnectorDefinition definition, CancellationToken ct)
    {
        Id = definition.Id;

        if (definition.Properties.TryGetValue("Root", out var root))
        {
            Root = root;
        }

        if (!Directory.Exists(root))
        {
            throw new InvalidOperationException($"Root directory '{root}' does not exist.");
        }

        return ValueTask.CompletedTask;
    }

    public async Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(DiscoveryFilter filter)
    {
        var path = Root;
        string searchPattern = "*";
        if (filter.Path is { })
        {
            path = Path.Combine(path, filter.Path);
        }

        if (filter.Filter is { })
        {
            searchPattern = $"*.{filter.Filter.ToLower()}";
        }

        SearchOption searchOption = filter.IncludeChildren
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        IEnumerable<string> files = Directory.EnumerateFiles(path, searchPattern, searchOption).Take(filter.MaxItems);

        IEnumerable<DiscoveredItem> items = files.Select(x =>
        {
            var file = new FileInfo(x);
            return BuildItemFromFile(file);
        });

        return items.ToList();
    }

    private DiscoveredItem BuildItemFromFile(FileInfo file)
    {
        return new DiscoveredItem
        {
            ConnectorId = Id,
            Id = GetRelativePath(file),
            ItemType = GetItemType(file),
            Name = Path.GetFileNameWithoutExtension(file.Name),
            CreatedAt = file.CreationTime
        };
    }

    public ValueTask<Stream> DownloadAsync(string id, CancellationToken ct)
    {
        Stream stream = File.OpenRead(GetFullPath(id));
        return ValueTask.FromResult(stream);
    }

    public async Task UploadAsync(string id, string path, Stream data, CancellationToken ct)
    {
        var newFolder = Path.Combine(Root, path);
        CreateDirectoryIfNotExists(newFolder);

        var newPath = Path.Combine(newFolder, id);

        await using FileStream fileStream = File.Create(newPath);
        await data.CopyToAsync(fileStream, ct);
    }

    public ValueTask DeleteAsync(string id, CancellationToken ct)
    {
        File.Delete(GetFullPath(id));

        return ValueTask.CompletedTask;
    }

    public ValueTask MoveAsync(string id, string path, CancellationToken ct)
    {
        var newFolder = Path.Combine(Root, path);
        CreateDirectoryIfNotExists(newFolder);

        string newPath = Path.Combine(newFolder, id);

        File.Move(GetFullPath(id), newPath);

        return ValueTask.CompletedTask;
    }

    private void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private string GetFullPath(params string[] paths)
    {
        return Path.Combine(
            new string[] { Root }.Concat(paths).ToArray());
    }

    private string GetRelativePath(FileInfo file)
    {
        return file.FullName.Replace(Root, "")
            .TrimStart([Path.DirectorySeparatorChar]);
    }

    private string? GetItemType(FileInfo file)
    {
        var extension = Path.GetExtension(file.Name).ToLower();
        if (extension is { })
        {
            var fileExtension = extension.Replace(".", "").ToLower();
            return ItemTypeMapper.GetFromFileExtension(fileExtension);
        }

        return null;
    }
}
