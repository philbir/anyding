
using Microsoft.Extensions.DependencyInjection;

namespace Anyding;

public static class ThingsServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddThings(this IAnydingServerBuilder builder)
    {
        builder.Services.AddScoped<IThingDataManager, ThingDataManager>();
        builder.Services.AddScoped<IThingsDataDownloader, ThingsDataDownloader>();
        builder.Services.AddScoped<IThingIngestionService, ThingIngestionService>();

        return builder;
    }
}
