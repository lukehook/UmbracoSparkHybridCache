using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;

public class PokemonService
{
    private HybridCache _cache;
    private PokeApiClient _pokeApiClient;

    public PokemonService(HybridCache cache, PokeApiClient pokeApiClient)
    {
        _cache = cache;
        _pokeApiClient = pokeApiClient;
    }

    public async Task<Pokemon> GetPokemonAsync(string name, string tag, CancellationToken token = default)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(1),
            LocalCacheExpiration = TimeSpan.FromMinutes(1)
        };

        var tagArray = Array.Empty<string>();
        if (string.IsNullOrEmpty(tag) == false)
        {
            tagArray = tag.Split('|');
        }

        return await _cache.GetOrCreateAsync(
            $"pokemon-{name}",
            async _ => await GetPokemonFromApiAsync(name, token),
            tags: tagArray
        );
    }

    private async Task<Pokemon> GetPokemonFromApiAsync(string name, CancellationToken token)
    {
        Pokemon pokemon = await _pokeApiClient.GetResourceAsync<Pokemon>(name);
        return pokemon;
    }
}
