using System.Collections;

namespace Anyding;

public sealed class EventCollection : IReadOnlyList<Event>
{
    private readonly List<Event> _events = [];

    public int Count => _events.Count;

    public Event this[int index] => _events[index];

    public void Add(Event domainEvent)
    {
        foreach (var @event in _events)
        {
            if (@event.Equals(domainEvent))
            {
                return;
            }
        }

        _events.Add(domainEvent);
    }


    public void Remove(Event domainEvent)
        => _events.Remove(domainEvent);

    public void Clear()
        => _events.Clear();

    public IEnumerator<Event> GetEnumerator()
        => _events.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
