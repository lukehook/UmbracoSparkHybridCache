using Microsoft.Extensions.Caching.Hybrid;
using PokeApiNet;

namespace SparkShared
{
    public class PokemonService
    {
        private HybridCache _cache;
        private PokeApiClient _pokeApiClient;

        public PokemonService(HybridCache hybridCache, PokeApiClient pokeApiClient)
        {
            _cache = hybridCache;
            _pokeApiClient = pokeApiClient;
        }

        public async Task<PokemonResponse> GetPokemonAsync(string name, string tag = "", bool fake = false, CancellationToken token = default)
        {

            #region EntryOptions
            var entryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(1),
                LocalCacheExpiration = TimeSpan.FromMinutes(1),
            };
            #endregion

            #region Tagging
            var tagArray = Array.Empty<string>();
            if (string.IsNullOrEmpty(tag) == false)
            {
                tagArray = tag.Split('|');
            }
            #endregion

            return await _cache.GetOrCreateAsync(
                $"pokemon-{name}",
                async _ => await GetPokemonFromApiAsync(name, token, fake),
                tags: tagArray,
                //options: entryOptions,
                cancellationToken: token
            );
        }

        public async Task<PokemonResponse> GetPokemonFromApiAsync(string name, CancellationToken token, bool fake = false)
        {
            Pokemon pokemon = await _pokeApiClient.GetResourceAsync<Pokemon>(name, token);

            if (fake)
            {
                throw new Exception("PokeApi down");
            }

            var response = new PokemonResponse
            {
                Name = pokemon.Name,
                Types = pokemon.Types.Select(t => t.Type.Name).ToArray(),
                Image = pokemon.Sprites.FrontDefault
            };

            return response;
        }
    }
}