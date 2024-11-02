using Anyding.Connectors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Anyding;

public class WorkspaceFactory(
    IOptions<FileSystemOptions> storageOptions,
    IEnumerable<IWorkspaceProvider> workspaceProviders,
    ILogger<WorkspaceFactory> logger) : IWorkspaceFactory
{
    public string RootDirectory => Path.Combine(storageOptions.Value.Root, storageOptions.Value.WorkspaceName);

    public async Task<IWorkspace> CreateNewWorkspaceAsync(
        Guid id,
        DiscoveredItem item,
        Stream stream,
        CancellationToken cancellationToken)
    {
        EnsureWorkspaceDirectoryExists();
        DirectoryInfo directory = GetWorkspaceDirectory(id);

        if (directory.Exists)
        {
            throw new InvalidOperationException($"Workspace already exists: {directory.FullName}");
        }

        directory.Create();
        var originalFilename = $"original{Path.GetExtension(item.Id)}";

        await CopyStreamAsync(stream, Path.Combine(directory.FullName, originalFilename), cancellationToken);

        WorkspaceInfo info = new()
        {
            Id = id,
            ItemType = item.ItemType,
            Discovery = item,
            Files =
            [
                new WorkspaceFile { Name = "Original", Path = originalFilename }
            ]
        };

        IWorkspace workspace = GetWorkspaceAsync(info, directory.FullName);
        await workspace.SaveInfoAsync(info, cancellationToken);

        return workspace;
    }

    private IWorkspace GetWorkspaceAsync(WorkspaceInfo info, string root)
    {
        foreach (IWorkspaceProvider provider in workspaceProviders)
        {
            if (provider.ManagedItemTypes.Contains(info.Discovery.ItemType))
            {
                return provider.CreateWorkspace(info.Id, root);
            }
        }

        return GetWorkspace(info.Discovery.ItemType, info.Id, root);
    }

    public async Task<IWorkspace> GetWorkspaceAsync(Guid id, CancellationToken ct)
    {
        DirectoryInfo directory = GetWorkspaceDirectory(id);
        if (!directory.Exists)
        {
            throw new InvalidOperationException($"Workspace with Id: {id} does not exist: {directory.FullName}");
        }

        WorkspaceInfo? info = await Workspace.GetInfoAsync(directory.FullName, ct);

        IWorkspace workspace = GetWorkspace(info.Discovery.ItemType, info.Id, directory.FullName);
        workspace.Info = info;

        return workspace;
    }

    private IWorkspace GetWorkspace(string itemType, Guid id, string root)
    {
        foreach (IWorkspaceProvider provider in workspaceProviders)
        {
            if (provider.ManagedItemTypes.Contains(itemType))
            {
                return provider.CreateWorkspace(id, root);
            }
        }

        return new Workspace(id, root);
    }

    private async Task CopyStreamAsync(Stream stream, string path, CancellationToken cancellationToken)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    private void EnsureWorkspaceDirectoryExists()
    {
        if (!Directory.Exists(RootDirectory))
        {
            Directory.CreateDirectory(RootDirectory);
        }
    }

    public DirectoryInfo GetWorkspaceDirectory(Guid id) =>
        new(Path.Combine(RootDirectory, id.ToString("N")));
}
