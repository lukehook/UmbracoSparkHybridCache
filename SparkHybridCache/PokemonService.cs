using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;

public class PokemonService(HybridCache cache, PokeApiClient pokeApiClient)
{
    private HybridCache _cache = cache;
    private PokeApiClient _pokeApiClient = pokeApiClient;

    public async Task<Pokemon> GetPokemonAsync(string name, CancellationToken token = default)
    {
        //var entryOptions = new HybridCacheEntryOptions
        //{
        //    Expiration = TimeSpan.FromMinutes(1),
        //    LocalCacheExpiration = TimeSpan.FromMinutes(1)
        //};

        return await _cache.GetOrCreateAsync(
            $"pokemon-{name}",
            async _ => await GetPokemonFromApiAsync(name, token)
            );
    }

    private async Task<Pokemon> GetPokemonFromApiAsync(string name, CancellationToken token)
    {
        Pokemon pokemon = await _pokeApiClient.GetResourceAsync<Pokemon>(name);
        return pokemon;
    }
}