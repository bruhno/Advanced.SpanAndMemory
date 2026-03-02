using Microsoft.Extensions.ObjectPool;
using System.Collections.Concurrent;

var random = new Random();

using var cts = new CancellationTokenSource();

var tasks = Enumerable.Range(1, 30).Select(_ => Task.Run(async () =>
{
    while (true)
    {
        cts.Token.ThrowIfCancellationRequested();

        var worker = SimpleObjectPool<Worker>.Instance.Get();
        try
        {
            await Task.Delay(random.Next(100, 300));
            Console.WriteLine($"worker id = {worker.Id}, reuse count = {worker.ReuseCounter}");
        }
        finally
        {
            SimpleObjectPool<Worker>.Instance.Return(worker);
        }

        await Task.Delay(random.Next(50, 100));
    }
}, cts.Token));

cts.CancelAfter(1000);

await Task.WhenAll(tasks);

class Worker: IResettable
{
    public int Id { get; } = Interlocked.Increment(ref _counter);

    public int ReuseCounter { get; private set; }

    static int _counter;

    public bool TryReset()
    {
        ReuseCounter++;
        return true;
    }
}

class SimpleObjectPool<T>: ObjectPool<T> where T : class, new()
{
    public static SimpleObjectPool<T> Instance { get; } = new();

    public override T Get()
    {
        if (_stack.TryPeek(out var obj))
        {
            return obj;
        }
        else
        {
            return new T();
        }
    }

    public override void Return(T obj)
    {
        if (obj is IResettable resettable)
        {
            _ = resettable.TryReset();
        }

        _stack.Push(obj);
    }

    private ConcurrentStack<T> _stack = new();
}