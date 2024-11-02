namespace Anyding.Events;

public record WorkspacePipelineTaskCompletedEvent(Guid WorkspaceId, string TaskName) : Event;

public record WorkspacePipelineTaskFailedEvent(Guid WorkspaceId, string TaskName, string Error) : Event;

public record WorkspacePipelineCompletedEvent(Guid WorkspaceId) : Event;
