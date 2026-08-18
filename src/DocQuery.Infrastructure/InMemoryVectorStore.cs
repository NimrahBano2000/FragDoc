using DocQuery.Core;

namespace DocQuery.Infrastructure
{
    public class InMemoryVectorStore : IVectorStore
    {
        private record Entry(string DocumentName, string ChunkText, float[] Embedding);
        private readonly List<Entry> _entries = new();

        public void Add(string documentName, IReadOnlyList<string> chunks, IReadOnlyList<float[]> embeddings)
        {
            if (chunks.Count != embeddings.Count)
            {
                throw new ArgumentException("Chunks and embeddings must have the same count.");
            }
            for (int i = 0; i < chunks.Count; i++)
            {
                _entries.Add(new Entry(documentName, chunks[i], embeddings[i]));
            }
        }

        public IReadOnlyList<(string Chunk, string DocumentName, double Score)> Search(float[] queryEmbedding, int topK)
        {
            return _entries.Select(e => (e.ChunkText, e.DocumentName, Score: VectorMath.CosineSimilarity(queryEmbedding, e.Embedding)))
                                        .OrderByDescending(e => e.Score)
                                        .Take(topK)
                                        .ToList();
        }
    }
}
