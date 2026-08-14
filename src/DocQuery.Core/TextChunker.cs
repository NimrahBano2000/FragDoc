namespace DocQuery.Core;

public class TextChunker
{
    private readonly int _chunkSize;
    private readonly int _overlapSize;

    public TextChunker() : this(1500, 200) { }
    public TextChunker(int chunkSize, int overlapSize)
    {
        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be a positive integer.");
        }
        if(overlapSize < 0 || overlapSize >= chunkSize)
        {
            throw new ArgumentOutOfRangeException(nameof(overlapSize), "Overlap size must be a positive integer less than chunk size.");
        } 
        _chunkSize = chunkSize;
        _overlapSize = overlapSize;
        
    }
    public List<string> ChunkText(string text)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }
        List<string> chunks = new List<string>();
        int startIndex = 0;
        while (startIndex < text.Length)
        {
            int length = Math.Min(_chunkSize, text.Length - startIndex);
            string chunk = text.Substring(startIndex, length);
            chunks.Add(chunk);
            startIndex += _chunkSize - _overlapSize;
        }
        return chunks;
    }


}
