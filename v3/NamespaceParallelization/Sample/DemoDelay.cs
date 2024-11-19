using System;
using System.Threading.Tasks;

namespace NamespaceParallelization.Sample;

public static class DemoDelay
{
    static readonly Random random = new();

    public static Task WaitFixedTimeAsync(int milliseconds)
        => Task.Delay(milliseconds);

    public static Task WaitRandomTimeAsync()
        => Task.Delay(random.Next(500, 5000));
}
