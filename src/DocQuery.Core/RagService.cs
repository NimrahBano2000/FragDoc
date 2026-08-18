

using System.Text;

namespace DocQuery.Core
{
    public class RagService
    {
        private readonly TextChunker _chunker;
        private readonly IEmbeddingClient _embeddings;
        private readonly IChatClient _chat;
        private readonly IVectorStore _store;

        public RagService(TextChunker textChunker, IEmbeddingClient embeddingClient, IChatClient chatClient, IVectorStore vectorStore)
        {
            _chunker = textChunker;
            _embeddings = embeddingClient;
            _chat = chatClient;
            _store = vectorStore;
        }
        public async Task IngestAsync(string documentName, string content, CancellationToken ct = default)
        {
             var chunks = _chunker.ChunkText(content);
             var vectors = await _embeddings.EmbedAsync(chunks, ct);
             _store.Add(documentName, chunks, vectors);
        }
        public async Task<string> AskAsync(string question, int topK = 4, CancellationToken ct = default)
        {
             var queryVector = (await _embeddings.EmbedAsync(new[] { question }, ct))[0];
             var hits = _store.Search(queryVector, topK);
             if (hits.Count == 0)
             {
                 return "No documents ingested yet.";
             }
            var sb = new StringBuilder();
            sb.AppendLine("Context:");
            for (int i = 0; i < hits.Count; i++)
            {
                sb.AppendLine($"[{i + 1}] ({hits[i].DocumentName})");
                sb.AppendLine(hits[i].Chunk);
                sb.AppendLine();
            }
            var systemPrompt = "You are an assistant that answers questions about documents." +
                " Answer using only the provided context. Cite the bracketed numbers for every claim." +
                " If the context doesn't contain the answer, say I don't know.";
             var userPrompt = $" {sb}\n\nQuestion: {question}";
             return await _chat.CompleteAsync(systemPrompt, userPrompt, ct);    
        }
    }
}
