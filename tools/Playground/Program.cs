using DocQuery.Core;

var text = """
This is the first paragraph. It talks about vacation policy and has enough words to be meaningful.

Second paragraph here. It discusses remote work rules across the company in some detail.

Third paragraph about parking, badges, and office access for all employees in the Munich office.
""";

var chunker = new TextChunker(chunkSize: 120, overlapSize: 30);
var chunks = chunker.ChunkText(text);

Console.WriteLine($"Total chunks: {chunks.Count}");
for (int i = 0; i < chunks.Count; i++)
{
    Console.WriteLine($"--- Chunk {i} (length {chunks[i].Length}) ---");
    Console.WriteLine(chunks[i]);
    Console.WriteLine();
}