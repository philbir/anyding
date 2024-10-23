namespace Anyding;

public interface IWorkspace
{
    Guid Id { get; set; }
    WorkspaceInfo Info { get; set; }
    string RootDirectory { get; set; }
    void Lock();
    void UnLock();
    T LoadFromJson<T>(string name);
    T LoadFromJson<T>(WorkspaceFile file);
    Stream LoadFileStream(WorkspaceFile file);
    Task InitializeAsync(CancellationToken cancellationToken);
    Task SaveAsJson<T>(string path, T data, CancellationToken ct);
    Task SaveAsJson<T>(WorkspaceFile file, T data, CancellationToken ct);
    Task SaveInfoAsync(WorkspaceInfo info, CancellationToken ct);
    Task SaveInfoAsync(CancellationToken ct);
    Task CreateWorkingCopyAsync(CancellationToken ct);
    void SetWorkingPath(string path);
    void AddFiles(params WorkspaceFile[] files);
    Task SaveFileAsync<T>(WorkspaceFile file, T data, CancellationToken ct);
    void AddPipelineResult(TaskExecutionResult result);
}
