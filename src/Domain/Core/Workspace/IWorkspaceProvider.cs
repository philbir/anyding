namespace Anyding;

public interface IWorkspaceProvider
{
    IWorkspace CreateWorkspace(Guid id, string directory);

    string[] ManagedItemTypes { get; }
}
