namespace Anyding;

public class ThingsWorkspaceHarvester(
    IEnumerable<IThingsWorkspaceHarvester> harvesters,
    IWorkspaceFactory workspaceFactory,
    IThingIngestionService thingIngestionService)
{
    public async Task RunAsync(Guid workspaceId, CancellationToken ct)
    {
        IWorkspace workspace = await workspaceFactory.GetWorkspaceAsync(workspaceId, ct);

        IThingsWorkspaceHarvester? harvester = harvesters.FirstOrDefault(
            x => x.MangedItemTypes.Contains(workspace.Info.ItemType));
        if (harvester == null)
        {
            throw new Exception("No harvester found for workspace");
        }

        CreateThingsRequest things = await harvester.ExecuteAsync(workspace, ct);
        await thingIngestionService.IngestAsync(things, ct);
    }
}

public interface IThingsWorkspaceHarvester
{
    string[] MangedItemTypes { get; }
    Task<CreateThingsRequest> ExecuteAsync(IWorkspace workspace, CancellationToken ct);
}

