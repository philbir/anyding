using Anyding.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Anyding.Ingest;

public class ThingIngestionPipeline(IServiceProvider provider)
{
    private readonly IDbContextFactory<AnydingDbContext> _dbContextFactory =
        provider.GetRequiredService<IDbContextFactory<AnydingDbContext>>();

    private readonly List<IThingIngestionMiddleware> _middlewares = new List<IThingIngestionMiddleware>();

    public ThingIngestionPipeline Use(IThingIngestionMiddleware middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    public ThingIngestionPipeline Use<TMiddleware>() where TMiddleware : IThingIngestionMiddleware
    {
        TMiddleware middleware = provider.GetRequiredService<TMiddleware>();
        _middlewares.Add(middleware);
        return this;
    }

    public async Task RunAsync(CreateThingsRequest request, CancellationToken ct)
    {
        var context = new ThingIngestionContext { Request = request, Cancelled = ct };
        context.Database = await _dbContextFactory.CreateDbContextAsync(ct);
        int index = -1;

        async Task Next()
        {
            index++;
            if (index < _middlewares.Count)
            {
                await _middlewares[index].InvokeAsync(context, Next);
            }
        }

        await Next();
    }
}
