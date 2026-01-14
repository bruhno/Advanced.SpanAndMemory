Random random = new Random();

var source = "The quick brown fox jumps over the lazy dog";

var destination = new char[9][];
for (int i = 0; i < destination.Length; i++)
{
    destination[i] = new char[10];
}

SplitByWords(source, destination);

WriteToConsole(destination);

void SplitByWords(string source, char[][] destination)
{
    var sourceSpan = source.AsMemory().Span;

    var spaceIndex = GetSpaceIndex(sourceSpan);

    var i = 0;

    while (spaceIndex >= 0)
    {
        var wordSpan = sourceSpan[..spaceIndex];

        if (wordSpan.Length > 0)
        {
            wordSpan.CopyTo(destination[i].AsMemory().Span);
            i++;
        }

        if (sourceSpan.Length <= spaceIndex + 1)
        {
            break;
        }

        sourceSpan = sourceSpan.Slice(spaceIndex + 1);

        spaceIndex = GetSpaceIndex(sourceSpan);
    }
}

int GetSpaceIndex(ReadOnlySpan<char> span)
{
    var spaceIndex = span.IndexOf(' ');

    return spaceIndex<0 
        ? span.Length
        : spaceIndex;
}

void WriteToConsole(char[][] arr)
{
    foreach (var word in arr)
    {
        foreach (var letter in word)
        {
            Console.Write(letter);
        }
        Console.Write(".");

        Console.WriteLine();
    }
}
