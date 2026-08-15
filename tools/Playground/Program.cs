using DocQuery.Core;
using DocQuery.Infrastructure;


var client = new OllamaEmbeddingClient(new LlmOptions());

//var text = """
//This is the first paragraph. It talks about vacation policy and has enough words to be meaningful.

//Second paragraph here. It discusses remote work rules across the company in some detail.

//Third paragraph about parking, badges, and office access for all employees in the Munich office.
//""";

var texts = new[]
{
    "Employees receive 30 vacation days per year and can carry over unused days.",
    "The database server is backed up nightly and indexes are rebuilt on Sundays.",
    "How many days of holiday do workers get?"
};


var chunker = new TextChunker(chunkSize: 120, overlapSize: 30);
var chunks = chunker.ChunkText(texts[0]);

Console.WriteLine($"Total chunks: {chunks.Count}");
for (int i = 0; i < chunks.Count; i++)
{
    Console.WriteLine($"--- Chunk {i} (length {chunks[i].Length}) ---");
    Console.WriteLine(chunks[i]);
    Console.WriteLine();
}

var vectors = await client.EmbedAsync(texts);

Console.WriteLine($"Vector length: {vectors[0].Length}");
Console.WriteLine($"Question vs vacation sentence: {VectorMath.CosineSimilarity(vectors[2], vectors[0]):F4}");
Console.WriteLine($"Question vs database sentence: {VectorMath.CosineSimilarity(vectors[2], vectors[1]):F4}");