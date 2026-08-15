namespace DocQuery.Infrastructure;

public class LlmOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434/v1";
    public string ApiKey { get; set; } = "ollama";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
    public string ChatModel { get; set; }   = "llama3.2";


}
