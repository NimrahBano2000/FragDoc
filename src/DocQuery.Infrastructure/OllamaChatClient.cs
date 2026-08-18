using DocQuery.Core;
using System.Net.Http.Json;
using System.Linq;

namespace DocQuery.Infrastructure
{
    public class OllamaChatClient: IChatClient
    {
        private readonly HttpClient _http;
        private readonly LlmOptions _options;

        public OllamaChatClient(LlmOptions options)
        {
            var handler = new HttpClientHandler { UseProxy = false };
            this._http = new HttpClient(handler);
            this._options = options;
        }

        public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
        {
            var response = await _http.PostAsJsonAsync(
                _options.BaseUrl.TrimEnd('/') + "/chat/completions",
                new
                {
                    model = _options.ChatModel,
                    messages = new[]
                    {
                            new { role = "system", content = systemPrompt },
                            new { role = "user", content = userPrompt }
                    }
                },
                ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Response {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            var payload = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken: ct);

            var completionText = payload?.Choices?.FirstOrDefault()?.Message?.Content ?? throw new InvalidOperationException("Empty chat response");
            return completionText;
        }

    }

    public class ChatResponse
    {
        public ChatChoice[] Choices { get; set; } = Array.Empty<ChatChoice>();
    }
    public class ChatChoice
    {
        public ChatMessage Message { get; set; } = new ChatMessage();
    }
    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
