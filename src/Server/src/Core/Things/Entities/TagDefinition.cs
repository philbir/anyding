namespace Anyding;

public class TagDefinition : Entity<Guid>
{
    public string Name { get; set; }

    public string? Color { get; set; }

    public string? Icon { get; set; }
}
