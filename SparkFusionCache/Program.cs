using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using PokeApiNet;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Duration = TimeSpan.FromSeconds(10),
        DistributedCacheDuration = TimeSpan.FromMinutes(10)
    })
    .WithSerializer(
        new FusionCacheSystemTextJsonSerializer()
    )
    .WithDistributedCache(
        new RedisCache(new RedisCacheOptions() { Configuration = builder.Configuration.GetConnectionString("RedisConnectionString") })
    )
    .AsHybridCache();

builder.Services.AddSingleton<PokeApiClient>();
builder.Services.AddSingleton<PokemonService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS policy
app.UseCors("OpenCorsPolicy");

app.UseStaticFiles();

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/pokemon/{name}", async (string name, PokemonService service, HttpRequest request) =>
{
    string types = request.Query["types"];
    return await service.GetPokemonAsync(name, types);
});

app.MapGet("/clear/{tag}", async (string tag, HybridCache cache) =>
{
    await cache.RemoveByTagAsync(tag);
    return Results.Ok();
});

app.Run();