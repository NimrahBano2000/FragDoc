using DocQuery.Core;
using DocQuery.Infrastructure;

var options = new LlmOptions();
var rag = new RagService(
    new TextChunker(chunkSize: 120, overlapSize: 30),
    new OllamaEmbeddingClient(options),
    new OllamaChatClient(options),
    new InMemoryVectorStore());

var handbook = """
This is the first paragraph. It talks about vacation policy: employees receive 30 vacation days per year.

Second paragraph here. It discusses remote work rules across the company in some detail.

Third paragraph about parking, badges, and office access for all employees in the Munich office.
""";

await rag.IngestAsync("handbook.md", handbook);

var answer = await rag.AskAsync("How many days of holiday do workers get?");
Console.WriteLine(await rag.AskAsync("What is the company's parental leave policy?"));
Console.WriteLine(answer);