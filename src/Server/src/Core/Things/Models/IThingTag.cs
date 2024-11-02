namespace Anyding;

public interface IThingTag
{
    Guid Id { get; set; }

    TagDefinition Definition { get; set; }

    string? Value { get; set; }
}
