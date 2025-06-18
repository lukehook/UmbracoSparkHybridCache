using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using PokeApiNet;
using System.Text.Json;

namespace SparkShared
{
    public class PokemonService
    {
        private HybridCache _cache;
        private PokeApiClient _pokeApiClient;

        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public PokemonService(HybridCache hybridCache, 
            PokeApiClient pokeApiClient,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache)
        {
            _cache = hybridCache;
            _pokeApiClient = pokeApiClient;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<PokemonResponse> GetPokemonAsync(string name, string tag = "", bool fake = false, CancellationToken token = default)
        {

            #region MemoryCache
            await _memoryCache.GetOrCreateAsync(
                name,
                async _ => await GetPokemonFromApiAsync(name, token, fake));
            #endregion

            #region DistributedCache
            await _distributedCache.GetAsync(name, token).ContinueWith(async task =>
            {
                if (task.Result == null)
                {
                    var pokemon = await GetPokemonFromApiAsync(name, token, fake);
                    await _distributedCache.SetAsync(name, JsonSerializer.SerializeToUtf8Bytes(pokemon), token);
                }
            }, token);
            #endregion

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

            #region HybridCache
            return await _cache.GetOrCreateAsync(
                $"pokemon-{name}",
                async _ => await GetPokemonFromApiAsync(name, token, fake),
                tags: tagArray,
                //options: entryOptions,
                cancellationToken: token
            );
            #endregion
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