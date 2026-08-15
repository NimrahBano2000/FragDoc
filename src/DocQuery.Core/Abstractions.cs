

namespace DocQuery.Core
{

        public interface IEmbeddingClient
        {
            Task<IReadOnlyList<float[]>> EmbedAsync(IReadOnlyList<string> inputs, CancellationToken ct = default);
        }
    
}
