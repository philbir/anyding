using Anyding.Data;

namespace Anyding;

public abstract class Entity
{
    public EventCollection Events { get; private set; } = [];
}

public abstract class Entity<TId> : Entity, IEntity<TId>
{
    public TId Id { get; set; }
}
