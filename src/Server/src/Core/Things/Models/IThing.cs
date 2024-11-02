namespace Anyding;

public interface IThing
{
    Guid Id { get; set; }

    string Name { get; set; }

    string? Description { get; set; }

    ThingType Type { get; set; }

    ThingClass? Class { get; set; }

    ThingSource? Source { get; set; }

    ThingState Status { get; set; }

    DateTimeOffset? Date { get; set; }

    DateTime? Created { get; set; }

    public List<ThingDataReference> Data { get; set; }
}
