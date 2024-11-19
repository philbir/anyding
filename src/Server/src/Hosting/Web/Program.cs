using Anyding;
using Anyding.Api;
using Anyding.Connector;
using Anyding.Query;
using Anyding.Search;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAnydingServer()
    .AddPostgresStore()
    //.AddMediatR()
    //.AddMassTransit()
    .AddThings()
    .AddConnectors()
    .AddWorkspaces()
    .AddTypeSense();

builder.Services.AddScoped<ThingLoader>();
builder.Services.AddMemoryCache();
builder.Services.AddMediatR(_ => _.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddAnydingApplication();

builder.Services
    .AddGraphQLServer()
    .AddAnydingGraphQL()
    .AddType<MediaThing>()
    .AddType<FaceThing>()
    .AddType<PersonThing>()
    .AddType<DeviceThing>()
    .AddType<AICaptionThingTag>()
    .AddType<AIColorThingTag>()
    .AddType<AIObjectThingTag>()
    .ModifyOptions(o =>
    {
        o.EnableDefer = true;
        o.EnableStream = true;
    });

    //.AddGlobalObjectIdentification();

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
ISearchDbContext searchDb = scope.ServiceProvider.GetRequiredService<ISearchDbContext>();

await searchDb.EnsureCreatedAsync();

app.UseMiddleware<AddCacheHeadersMiddleware>();
app.MapGroup("api/things/data").MapThingsDataApi()
    .WithTags("thingdata");

app.MapGraphQL();
app.Run();
