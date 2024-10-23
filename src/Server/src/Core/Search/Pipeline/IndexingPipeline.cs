using Anyding.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Anyding.Search;

public class IndexingPipeline(IServiceScopeFactory serviceScopeFactory, ILogger<IndexingPipeline> logger)
{
    private readonly List<Type> _middlewareTypes = new();

    public IndexingPipeline Use<TMiddleware>() where TMiddleware : IIndexingMiddleware
    {
        _middlewareTypes.Add(typeof(TMiddleware));
        return this;
    }

    public async Task RunAsync(IndexingRequest request, CancellationToken ct)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var middlewares = new List<IIndexingMiddleware>();
        foreach (Type mw in _middlewareTypes)
        {
            middlewares.Add((IIndexingMiddleware) serviceProvider.GetRequiredService(mw));
        }

        var context = new IndexingContext { Cancelled = ct, Request = request };
        context.Database = await serviceProvider
            .GetRequiredService<IDbContextFactory<AnydingDbContext>>()
            .CreateDbContextAsync(ct);

        SearchDbContext searchDb = serviceProvider.GetRequiredService<SearchDbContext>();

        int index = -1;
        async Task Next()
        {
            index++;
            if (index < middlewares.Count)
            {
                logger.LogInformation("Running middleware: {Middleware}", middlewares[index].GetType().Name);
                await middlewares[index].InvokeAsync(context, Next);
            }
        }

        await Next();

        searchDb.Persons.CreateDocuments(context.Persons.Values);
        searchDb.Media.CreateDocuments(context.Media.Values);
        searchDb.Faces.CreateDocuments(context.Faces.Values);
    }
}

public class IndexingRequest
{
    public List<IThing> Things { get; set; }
}

public interface IIndexingMiddleware
{
    Task InvokeAsync(IndexingContext context, Func<Task> next);
}

public static class IndexingServiceCollectionExtensions
{
    public static IServiceCollection AddIndexingPipeline(this IServiceCollection services)
    {
        services.AddScoped<PersonIndexingMiddleware>();
        services.AddScoped<MediaIndexingMiddleware>();

        services.AddScoped<IndexingPipeline>(p =>
        {
            var pipeline = new IndexingPipeline(
                p.GetRequiredService<IServiceScopeFactory>(),
                p.GetRequiredService<ILogger<IndexingPipeline>>());

            pipeline.Use<PersonIndexingMiddleware>();
            pipeline.Use<MediaIndexingMiddleware>();

            return pipeline;
        });

        return services;
    }
}
