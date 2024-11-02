namespace Anyding;

public class ThingTag : Entity<Guid>
{
    public Guid ThingId { get; set; }
    public TagDefinition Definition { get; set; }
    public Guid DefinitionId { get; set; }
    public string? Value { get; set; }
    public string Detail { get; set; }
}
