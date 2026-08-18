

namespace DocQuery.Core
{

    public interface IEmbeddingClient
    {
            Task<IReadOnlyList<float[]>> EmbedAsync(IReadOnlyList<string> inputs, CancellationToken ct = default);
    }

    public interface IChatClient
    {
        Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default);
    }
    public interface IVectorStore
    {
        void Add(string documentName, IReadOnlyList<string> chunks, IReadOnlyList<float[]> embeddings);
        IReadOnlyList<(string Chunk, string DocumentName, double Score)> Search(float[] queryEmbedding, int topK);
        
    }
}
