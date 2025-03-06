using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using PokeApiNet;
using SparkShared;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Duration = TimeSpan.FromSeconds(15),
        DistributedCacheDuration = TimeSpan.FromSeconds(30),

        #region EagerRefresh
        //EagerRefreshThreshold = 0.5f,
        #endregion

        #region FailSafe
        //IsFailSafeEnabled = true,
        //FailSafeMaxDuration = TimeSpan.FromHours(1),
        //FailSafeThrottleDuration = TimeSpan.FromSeconds(30)
        #endregion

    })
    .WithSerializer(
        new FusionCacheSystemTextJsonSerializer()
    )
    .WithDistributedCache(
        new RedisCache(
            new RedisCacheOptions()
            {
                Configuration = builder.Configuration.GetConnectionString("RedisConnectionString")
            }
        )
    )
    .AsHybridCache();



builder.Services.AddSingleton<PokeApiClient>();
builder.Services.AddSingleton<PokemonService>();



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
    string fake = request.Query["fake"];

    if (string.IsNullOrEmpty(fake) == false)
    {
        return await service.GetPokemonAsync(name, types, true);
    }

    return await service.GetPokemonAsync(name, types);
});

app.MapGet("/clear/{tag}", async (string tag, HybridCache cache) =>
{
    var tagArray = Array.Empty<string>();
    if (string.IsNullOrEmpty(tag) == false)
    {
        tagArray = tag.Split('|');
        foreach (var t in tagArray)
        {
            await cache.RemoveByTagAsync(t);
        }
    }

    return Results.Ok();
});

app.Run();