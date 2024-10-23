using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Anyding;

public class Workspace : IWorkspace
{
    JsonSerializerOptions IndentedJsonOptions => new() { WriteIndented = true };
    private const string _infoFileName = "info.json";
    private const string _lockFileName = "lock";
    private const string _originalFileName = "Original";
    private const string _workingFileName = "Working";

    public Workspace(Guid id, string rootDirectory)
    {
        Id = id;
        RootDirectory = rootDirectory;
    }

    public string OriginalPath => GetFileFullPath(_originalFileName);

    public string WorkingPath => GetFileFullPath(_workingFileName);

    public Guid Id { get; set; }

    public WorkspaceInfo Info { get; set; }

    public string RootDirectory { get; set; }

    public void Lock()
    {
        using var stream = new FileStream(
            Path.Combine(RootDirectory, _lockFileName),
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None);

        using var writer = new StreamWriter(stream);
        writer.WriteLine($"{DateTime.UtcNow:o}: {Environment.MachineName}:{Thread.CurrentThread.ManagedThreadId}");
    }

    private string GetFileFullPath(string name)
    {
        WorkspaceFile? file = Info.Files.FirstOrDefault(f => f.Name == name);
        if (file == null)
        {
            throw new InvalidOperationException($"File {name} not found in workspace {Id}");
        }

        return Path.Combine(RootDirectory, file.Path);
    }

    public void UnLock()
    {
        File.Delete(Path.Combine(RootDirectory, _lockFileName));
    }

    public T? LoadFromJson<T>(string name)
    {
        WorkspaceFile? file = Info.Files.FirstOrDefault(f => f.Name == name);
        if (file == null)
        {
            return default;
        }

        Stream stream = LoadFileStream(file);
        return JsonSerializer.Deserialize<T>(stream);
    }

    public T LoadFromJson<T>(WorkspaceFile file)
    {
        return LoadFromJson<T>(file.Name);
    }

    public Stream LoadFileStream(WorkspaceFile file)
    {
        RetryPolicy? retryPolicy = Policy
            .Handle<IOException>()
            .WaitAndRetry(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
            });

        Stream stream = retryPolicy.Execute(() => new FileStream(
            Path.Combine(RootDirectory, file.Path),
            FileMode.Open,
            FileAccess.Read));

        return stream;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        Info = await GetInfoAsync(cancellationToken);
    }

    public Task SaveAsJson<T>(WorkspaceFile file, T data, CancellationToken ct)
    {
        return SaveFileAsync(file, data, ct);
    }

    public async Task SaveAsJson<T>(string path, T data, CancellationToken ct)
    {
        await using var stream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        await JsonSerializer.SerializeAsync(stream, data, IndentedJsonOptions, ct);
    }

    public async Task SaveInfoAsync(WorkspaceInfo info, CancellationToken ct)
    {
        await SaveAsJson(Path.Combine(RootDirectory, _infoFileName), info, ct);
    }

    public async Task SaveInfoAsync(CancellationToken ct)
    {
        await SaveAsJson(Path.Combine(RootDirectory, _infoFileName), Info, ct);
    }

    public async Task CreateWorkingCopyAsync(CancellationToken ct)
    {
        var workingFilename = $"working.{OriginalPath.GetFileExtensionWithoutDot()}";

        File.Copy(OriginalPath, Path.Combine(RootDirectory, workingFilename), overwrite: true);

        Info.Files.Add(new() { Name = _workingFileName, Path = workingFilename });

        await SaveInfoAsync(ct);
    }

    public void SetWorkingPath(string path)
    {
        WorkspaceFile? file = Info.Files.FirstOrDefault(x => x.Name == _workingFileName);
        if (file != null)
        {
            file.Path = path;
        }
    }

    private Task<WorkspaceInfo?> GetInfoAsync(CancellationToken cancellationToken)
    {
        return GetInfoAsync(Path.Combine(RootDirectory, _infoFileName), cancellationToken);
    }

    internal static async Task<WorkspaceInfo?> GetInfoAsync(string directory, CancellationToken cancellationToken)
    {
        await using Stream stream = new FileStream(
            Path.Combine(directory, _infoFileName),
            FileMode.Open,
            FileAccess.Read);

        return JsonSerializer.Deserialize<WorkspaceInfo>(stream);
    }

    internal static bool IsLocked(string directory)
    {
        return File.Exists(Path.Combine(directory, _lockFileName));
    }

    public void AddResult(TaskExecutionResult result)
    {
        var results = Info.PipelineResult;
        var existing = results.FindIndex(r => r.Name == result.Name);
        if (existing > -1)
        {
            if (!result.Success)
            {
                result.RetryCount = results[existing].RetryCount + 1;
            }

            results[existing] = result;
        }
        else
        {
            results.Add(result);
        }
    }

    public void AddFiles(params WorkspaceFile[] files)
    {
        Info.Files.AddRange(files);
    }

    public async Task SaveFileAsync<T>(WorkspaceFile file, T data, CancellationToken ct)
    {
        var location = Path.Combine(RootDirectory, file.Path);

        if (data is Stream stream)
        {
            file.ContentType = Path.GetExtension(file.Path);
            await SaveAsync(location, stream, ct);
        }
        else if (data is byte[] bytes)
        {
            file.ContentType = Path.GetExtension(file.Path);
            await SaveAsync(location, bytes, ct);
        }
        else
        {
            file.ContentType = $".json/{typeof(T).FullName}";
            SaveAsJson(location, data, ct);
        }

        Info.Files.Add(file);
    }

    public void AddPipelineResult(TaskExecutionResult result)
    {
        Info.PipelineResult.Add(result);
    }

    private async Task SaveAsync(string path, Stream data, CancellationToken cancellationToken)
    {
        await using var fileStream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        await data.CopyToAsync(fileStream, cancellationToken);
    }

    private async Task SaveAsync(string path, byte[] data, CancellationToken ct)
    {
        await File.WriteAllBytesAsync(path, data, ct);
    }
}
