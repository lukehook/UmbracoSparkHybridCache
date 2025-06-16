using MessagePack;
using MessagePack.Resolvers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion.Internals;
using ZiggyCreatures.Caching.Fusion.Serialization;

namespace SparkUmbracoFusionCache
{
    public class FusionCacheUmbracoContentCacheSerializer : IFusionCacheSerializer
    {

        public class Options
        {
            public MessagePackSerializerOptions? SerializerOptions { get; set; }
        }

        private readonly MessagePackSerializerOptions? _serializerOptions;

        public FusionCacheUmbracoContentCacheSerializer(MessagePackSerializerOptions? options = null)
        {
            MessagePackSerializerOptions defaultOptions = ContractlessStandardResolver.Options;
            IFormatterResolver resolver = CompositeResolver.Create(defaultOptions.Resolver);

            _serializerOptions = options ?? defaultOptions
                .WithResolver(resolver)
                .WithCompression(MessagePackCompression.Lz4BlockArray)
                .WithSecurity(MessagePackSecurity.UntrustedData);
        }

        public FusionCacheUmbracoContentCacheSerializer(Options? options)
            : this(options?.SerializerOptions)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Serialize<T>(T? obj)
        {
            using ArrayPoolBufferWriter arrayPoolBufferWriter = new ArrayPoolBufferWriter();
            MessagePackSerializer.Serialize(arrayPoolBufferWriter, obj, _serializerOptions);
            return arrayPoolBufferWriter.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? Deserialize<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data.AsMemory(), _serializerOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<byte[]> SerializeAsync<T>(T? obj, CancellationToken token = default(CancellationToken))
        {
            return new ValueTask<byte[]>(Serialize(obj));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<T?> DeserializeAsync<T>(byte[] data, CancellationToken token = default(CancellationToken))
        {
            return new ValueTask<T>(Deserialize<T>(data));
        }

        public override string ToString()
        {
            return GetType().Name ?? "";
        }
    }
}