using System;
using Xunit;

// Attach this type as a fixture. This will be created one for the test assembly and shared for
// any test which wants to get access to it (via constructor).
//
// All fixtures can optionally implement IDisposable, IAsyncDisposable, or IAsyncLifetime in order
// get full lifetime control.
[assembly: AssemblyFixture(typeof(MyAssemblyFixture))]

public class Sample1(MyAssemblyFixture fixture)
{
    [Fact]
    public void EnsureSingleton() =>
        Assert.Equal(2, fixture.InstantiationCount);
}

public class Sample2(MyAssemblyFixture fixture)
{
    [Fact]
    public void EnsureSingleton() =>
        Assert.Equal(2, fixture.InstantiationCount);
}

public class MyAssemblyFixture : IDisposable
{
    static int instantiationCount;

    public MyAssemblyFixture() =>
        instantiationCount++;

    public int InstantiationCount =>
        instantiationCount;

    public void Dispose()
    {
        // Uncomment this and it will surface as an assembly cleanup failure
        //throw new DivideByZeroException();
    }
}
