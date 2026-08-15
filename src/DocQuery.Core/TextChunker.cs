
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

        // 2. split into paragraphs
        string[] paragraphs = text.ReplaceLineEndings("\n").Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


        var safeParagraphs = new List<string>();
        foreach (var p in paragraphs)
        {
            if (p.Length > _chunkSize) safeParagraphs.AddRange(SplitBySize(p));
            else safeParagraphs.Add(p);
        }

        var chunks = new List<string>();
        var current = new System.Text.StringBuilder();
        foreach (var p in safeParagraphs)
        {
            bool wouldOverflow = current.Length > 0 && current.Length + p.Length + 2 > _chunkSize;
            if (wouldOverflow)
            {
                string closed = current.ToString();
                chunks.Add(closed);
                current.Clear();
                string seed = closed.Length >= _overlapSize ? closed.Substring(closed.Length - _overlapSize) : closed;
                current.Append(seed);
            }
            if (current.Length > 0) current.Append("\n\n");
            current.Append(p);
        }
        if (current.Length > 0)
        {
            chunks.Add(current.ToString());
        }

        return chunks;

    }
    private List<string> SplitBySize(string paragraph)
    {
        List<string> chunks = new List<string>();
        int startIndex = 0;
        while (startIndex < paragraph.Length)
        {
            int length = Math.Min(_chunkSize, paragraph.Length - startIndex);
            string chunk = paragraph.Substring(startIndex, length);
            chunks.Add(chunk);
            startIndex += _chunkSize - _overlapSize;
        }
        return chunks;
    }


}
