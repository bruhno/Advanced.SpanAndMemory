
using Newtonsoft.Json;
using System.Buffers;

var data = "[1,2,3]";

var arr = ReadData(data);

foreach (var item in arr)
{
    Console.WriteLine(item);
}

static int[] ReadData(string data)
{
    var serializer = new JsonSerializer();

    using var reader = new JsonTextReader(new StringReader(data));

    reader.ArrayPool = new MyArrayPool();

    var value  = serializer.Deserialize<int[]>(reader);

    return value;
}

internal class MyArrayPool: IArrayPool<char>
{
    public char[] Rent(int minimumLength)
    {
        return ArrayPool<char>.Shared.Rent(minimumLength);
    }

    public void Return(char[]? array)
    {
        if (array is not null)
        {
            ArrayPool<char>.Shared.Return(array);
        }
    }
}