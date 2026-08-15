using DocQuery.Core;
using System.Net.Http.Json;
using System.Linq;

namespace DocQuery.Infrastructure
{
    public class OllamaEmbeddingClient : IEmbeddingClient
    {
        private readonly HttpClient _http;
        private readonly LlmOptions _options;

        public OllamaEmbeddingClient(LlmOptions options)
        {
            var handler = new HttpClientHandler { UseProxy = false };
            this._http = new HttpClient(handler);
            this._options = options;
        }

        public async Task<IReadOnlyList<float[]>> EmbedAsync(IReadOnlyList<string> inputs, CancellationToken ct = default)
        {
            var response = await _http.PostAsJsonAsync(
                _options.BaseUrl.TrimEnd('/') + "/embeddings",
                new { model = _options.EmbeddingModel, input = inputs },
                ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Response {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            // Deserialize once into a typed model
            var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: ct);

            var embeddings = payload?.Data?.OrderBy(d => d.Index).Select(d => d.Embedding).ToArray() ?? throw new InvalidOperationException("Empty embedding response");
            return embeddings;
        }
    }

    // Add (or move to a shared Models namespace) a response model that matches the API.
    public class EmbeddingResponse
    {
        public EmbeddingItem[] Data { get; set; } = Array.Empty<EmbeddingItem>();
    }

    public class EmbeddingItem
    {
        public int Index { get; set; }
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
