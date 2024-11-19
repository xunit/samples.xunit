using System;
using System.Diagnostics;
using Xunit;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

public class TestOutputExamples(ITestOutputHelper output)
{
    // You can take ITestOutputHelper as a dependency...
    [Fact]
    public void ViaOutputHelper()
    {
        output.WriteLine("I'm inside the test!");
    }

    // ...or you can use the text context ...
    [Fact]
    public void ViaTestContext()
    {
        TestContext.Current.TestOutputHelper?.WriteLine("I'm inside another test!");
    }

    // ...or via console capture...
    [Fact]
    public void ViaConsole()
    {
        Console.WriteLine("I'm writing to the console now...");
    }

    // ...or via Trace/Debug
    [Fact]
    public void ViaTrace()
    {
        Trace.WriteLine("I'm writing a trace message");
    }
}
