namespace Anyding.Search;

public class PersonIndexingMiddleware : IIndexingMiddleware
{
    private readonly SearchDbContext _searchDb;

    public PersonIndexingMiddleware(SearchDbContext searchDb)
    {
        _searchDb = searchDb;
    }

    public async Task InvokeAsync(IndexingContext context, Func<Task> next)
    {
        foreach (PersonThing thing in context.Request.Things.Where(x => x is PersonThing).ToList())
        {
            var personIndex = new PersonIndex
            {
                Id = thing.Id.ToId(), Name = thing.Name, DateOfBirth = thing.Details.DateOfBirth.ToUnixTimeSeconds()
            };

            context.AddPerson(personIndex);
        }

        await next();
    }
}
