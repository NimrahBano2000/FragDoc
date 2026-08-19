# FragDoc — Document Q&A with Retrieval-Augmented Generation (.NET 8)

[![CI](https://github.com/NimrahBano2000/FragDoc/actions/workflows/ci.yml/badge.svg)](https://github.com/NimrahBano2000/FragDoc/actions)

Ask questions about your own documents and get answers **grounded in the source text, with citations** — or an honest "I don't know" when the documents don't contain the answer. Built from scratch in .NET 8 (no LangChain, no RAG framework) to understand the pattern at the implementation level.

## How it works

**Ingest:** documents are split into paragraph-aware chunks with overlap → each chunk is embedded via `nomic-embed-text` (Ollama) → vectors stored in an in-memory store.

**Ask:** the question is embedded the same way → top-K chunks retrieved by cosine similarity → passed to an LLM (`llama3.2`) with strict instructions: answer only from the context, cite sources as [1], [2], say so if the answer isn't there.

## Architecture
src/DocQuery.Core domain: chunker, cosine similarity, RAG orchestration, interfaces
src/DocQuery.Infrastructure adapters: Ollama HTTP clients, in-memory vector store
src/DocQuery.Api ASP.NET Core Minimal API + Swagger
tests/DocQuery.Tests xUnit test suite
tools/Playground console harness for end-to-end experiments


Core depends on nothing and defines the interfaces (`IEmbeddingClient`, `IChatClient`, `IVectorStore`); Infrastructure implements them. The LLM endpoint is OpenAI-compatible, so switching from local Ollama to OpenAI or Azure OpenAI is configuration, not code.

## Run it

**Prerequisites:** [Ollama](https://ollama.com) with `ollama pull nomic-embed-text` and `ollama pull llama3.2`.

**Locally:**
```bash
dotnet run --project src/DocQuery.Api
# open http://localhost:5111/swagger
```

**With Docker:**
```bash
docker build -t fragdoc .
docker run -p 8080:8080 -e Llm__BaseUrl=http://host.docker.internal:11434/v1 fragdoc
# open http://localhost:8080/swagger
```

## API

| Endpoint | Description |
|---|---|
| `POST /api/documents` | Ingest a document (`{ "name": "...", "content": "..." }`) |
| `POST /api/ask` | Ask a question (`{ "question": "...", "topK": 4 }`) |
| `GET /health` | Health check |

**Example** — after ingesting an HR handbook:

Request: `{ "question": "How many days of holiday do workers get?" }`
Response: `{ "answer": "30 [1]" }`

Asked about a policy *not* in the documents, it refuses instead of hallucinating: `"I don't know."`

## Tests

```bash
dotnet test
```

Covers chunking (boundaries, overlap, oversized paragraphs) and cosine similarity (identical, orthogonal, opposite, zero, and scaled vectors) — run in CI on every push.

## Design decisions

- **Paragraph-aware chunking with overlap** — chunks hold complete thoughts, and facts near boundaries survive in a neighboring chunk.
- **In-memory brute-force vector store** — right-sized for the corpus; lives behind `IVectorStore` so pgvector/Qdrant can be swapped in without touching domain code.
- **Singleton store, stateless services** — DI lifetimes chosen deliberately (the store must outlive a request).
- **No framework** — chunking, retrieval, and orchestration are hand-written to keep every step inspectable.

## Roadmap

File upload endpoint · PDF ingestion · persistent vector store (Postgres + pgvector) · streaming answers
