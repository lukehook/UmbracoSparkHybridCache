using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("RedisConnectionString");
});

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache(options =>
    {
        options.MaximumPayloadBytes = 1024 * 1024;
        options.MaximumKeyLength = 1024;
        options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromSeconds(5)
        };
    });
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Services.AddSingleton<PokemonService>();
builder.Services.AddSingleton<PokeApiClient>();

var app = builder.Build();

app.MapGet("/pokemon/{name}", async (string name, PokemonService service) =>
    {
        return await service.GetPokemonAsync(name);
    });

app.Run();