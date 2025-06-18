using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;
using SparkShared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Replace this with your own Redis connection string in appsettings.json or you
    // you can swap this out for a different IDistributedCache implementation such as SQL Server
    options.Configuration =
        builder.Configuration.GetConnectionString("RedisConnectionString");
});


#region HybridCache
builder.Services.AddHybridCache(options =>
    {
        options.MaximumPayloadBytes = 1024 * 1024;
        options.MaximumKeyLength = 1024;
        options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromSeconds(30),
            LocalCacheExpiration = TimeSpan.FromSeconds(15)
        };
    });
#endregion


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

app.MapGet("/pokemon/{name}", async (string name, PokemonService service) =>
    {
        return await service.GetPokemonAsync(name);
    });

app.Run();