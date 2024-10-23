using Anyding;
using Anyding.Query;
using Anyding.Search;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAnydingServer()
    .AddPostgresStore()
    .AddTypeSense();

builder.Services.AddSingleton<ThingLoader>();

builder.Services
    .AddGraphQLServer()
    .AddAnydingGraphQL()
    .AddType<MediaThing>()
    .AddType<FaceThing>()
    .AddType<PersonThing>()
    .AddType<DeviceThing>()
    .AddType<AICaptionThingTag>()
    .AddType<AIColorThingTag>()
    .AddType<AIObjectThingTag>();
    //.AddGlobalObjectIdentification();

WebApplication app = builder.Build();

SearchDbContext searchDb = app.Services.GetRequiredService<SearchDbContext>();
await searchDb.EnsureCreatedAsync();

app.MapGraphQL();
app.Run();
