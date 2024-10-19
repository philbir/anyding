using Microsoft.Extensions.Options;

namespace Anyding;

public interface IFileSystemStore
{
    DirectoryInfo Workspace { get; }
}

public class FileSystemStore(IOptions<FileStorageOptions> options) : IFileSystemStore
{
    public DirectoryInfo Workspace => new(Path.Combine(options.Value.Root, options.Value.StagingAreaName));
}

public class FileStorageOptions
{
    public string Root { get; set; }

    public string StagingAreaName { get; set; } = "workspace";
}


internal static class FileExtension
{
    public static string GetFileExtensionWithoutDot(this string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (!string.IsNullOrEmpty(extension) && extension[0] == '.')
        {
            return extension.AsSpan(1).ToString();
        }

        return extension;
    }
}
