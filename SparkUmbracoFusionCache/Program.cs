using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NeueccMessagePack;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

// Remove all registrations of HybridCache from the service collection
builder.Services.RemoveAll(descriptor => descriptor.ServiceType == typeof(HybridCache));

builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Size = 1024 * 1024 * 100,
    })
    .WithSerializer(
        new FusionCacheNeueccMessagePackSerializer()
    )
    .WithDistributedCache(
        new RedisCache(
            new RedisCacheOptions()
            {
                // Replace this with your own Redis connection string in appsettings.json or you
                // you can swap this out for a different IDistributedCache implementation such as SQL Server
                Configuration = builder.Configuration.GetConnectionString("RedisConnectionString")
            }
        )
    )
    .AsHybridCache();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
