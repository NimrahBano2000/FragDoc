using DocQuery.Core;
using DocQuery.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var llmOptions = builder.Configuration.GetSection("Llm").Get<LlmOptions>() ?? new LlmOptions();
builder.Services.AddSingleton(llmOptions);
builder.Services.AddSingleton<TextChunker>();
builder.Services.AddSingleton<IEmbeddingClient, OllamaEmbeddingClient>();
builder.Services.AddSingleton<IChatClient, OllamaChatClient>();
builder.Services.AddSingleton<IVectorStore, InMemoryVectorStore>();
builder.Services.AddScoped<RagService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/documents", async (IngestRequest req, RagService rag, CancellationToken ct) =>
{
    if (req.Name is null || req.Content is null)
    {
        return Results.BadRequest(new { error = "Name and Content are required." });
    }

    await rag.IngestAsync(req.Name, req.Content, ct).ConfigureAwait(false);
    return Results.Ok(new { message = $"Ingested '{req.Name}'." });
});

app.MapPost("/api/ask", async (AskRequest req, RagService rag, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(req.Question))
    {
        return Results.BadRequest(new { error = "Question is required." });
    }

    var answer = await rag.AskAsync(req.Question, req.TopK ?? 4, ct).ConfigureAwait(false);
    return Results.Ok(new { answer });
});

app.MapGet("/", () => "Hello World!");

app.Run();


public sealed record IngestRequest(string Name, string Content);

public sealed record AskRequest(string Question, int? TopK);
