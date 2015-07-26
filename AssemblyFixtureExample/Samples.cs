using System;
using Xunit;

// The custom test framework enables the support
[assembly: TestFramework("AssemblyFixtureExample.XunitExtensions.XunitTestFrameworkWithAssemblyFixture", "AssemblyFixtureExample")]

// Add one of these for every fixture classes for the assembly.
// Just like other fixtures, you can implement IDisposable and it'll
// get cleaned up at the end of the test run.
[assembly: AssemblyFixture(typeof(MyAssemblyFixture))]

public class Sample1
{
    MyAssemblyFixture fixture;

    // Fixtures are injectable into the test classes, just like with class and collection fixtures
    public Sample1(MyAssemblyFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void EnsureSingleton()
    {
        Assert.Equal(1, MyAssemblyFixture.InstantiationCount);
    }
}

public class Sample2
{
    MyAssemblyFixture fixture;

    public Sample2(MyAssemblyFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void EnsureSingleton()
    {
        Assert.Equal(1, MyAssemblyFixture.InstantiationCount);
    }
}

public class MyAssemblyFixture : IDisposable
{
    public static int InstantiationCount;

    public MyAssemblyFixture()
    {
        InstantiationCount++;
    }

    public void Dispose()
    {
        // Uncomment this and it will surface as an assembly cleanup failure
        //throw new DivideByZeroException();
    }
}
