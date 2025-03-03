using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;
using SparkShared;



public class PokemonService(HybridCache cache, PokeApiClient pokeApiClient)
{
    private HybridCache _cache = cache;
    private PokeApiClient _pokeApiClient = pokeApiClient;




    public async Task<PokemonResponse> GetPokemonAsync(string name, CancellationToken token = default)
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




    private async Task<PokemonResponse> GetPokemonFromApiAsync(string name, CancellationToken token)
    {
        Pokemon pokemon = await _pokeApiClient.GetResourceAsync<Pokemon>(name, token);

        var response = new PokemonResponse
        {
            Name = pokemon.Name,
            Types = pokemon.Types.Select(t => t.Type.Name).ToArray(),
            Image = pokemon.Sprites.FrontDefault
        };

        return response;
    }





}