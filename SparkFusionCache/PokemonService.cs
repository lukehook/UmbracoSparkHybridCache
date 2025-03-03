using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;
using SparkShared;

public class PokemonService
{
    private HybridCache _cache;
    private PokeApiClient _pokeApiClient;

    public PokemonService(HybridCache cache, PokeApiClient pokeApiClient)
    {
        _cache = cache;
        _pokeApiClient = pokeApiClient;
    }

    public async Task<PokemonResponse> GetPokemonAsync(string name, string tag, CancellationToken token = default)
    {
        #region Tagging
        var tagArray = Array.Empty<string>();
        if (string.IsNullOrEmpty(tag) == false)
        {
            tagArray = tag.Split('|');
        }
        #endregion

        var cacheEntry = await _cache.GetOrCreateAsync(
            $"pokemon-{name}",
            async _ => await GetPokemonFromApiAsync(name, token),
            tags: tagArray,
            cancellationToken: token
        );

        return (cacheEntry);

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
