using System.Buffers;

var s1 = new MySegment(new int[] { 1, 2, 3 });
s1.SetNext(new(new int[] { 4, 5, 6 }));

var sequence = new ReadOnlySequence<int>(s1, 0, s1.Next!, s1.Next!.Memory.Length);

sequence = sequence.Slice(0, 5);

foreach (var segment in sequence)
{
    foreach (var item in segment.Span)
    {
        Console.WriteLine(item);
    }
    Console.WriteLine("---");
}

class MySegment: ReadOnlySequenceSegment<int>
{
    public MySegment(ReadOnlyMemory<int> memory)
    {
        Memory = memory;
    }

    public void SetNext(MySegment segment)
    {
        Next = segment;
        segment.RunningIndex = RunningIndex + Memory.Length;
    }
}