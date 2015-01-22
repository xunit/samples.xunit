using System;
using Xunit;
using Xunit.Abstractions;

public class Example
{
    ITestOutputHelper output;

    public Example(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestThis()
    {
        output.WriteLine("I'm inside the test!");
    }
}