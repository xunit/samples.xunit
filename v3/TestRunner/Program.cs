using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Runners;

namespace TestRunner;

// This builds for both .NET Framework and .NET.
//
// The .NET Framework version can run v1/v2/v3 .NET Framework tests, and v3 .NET tests.
// The .NET version can run v3 .NET Framework tests, and v3 .NET tests.
class Program
{
    // We use consoleLock because messages can arrive in parallel, so we want to make sure we get
    // consistent console output.
    static readonly object consoleLock = new();

    // Use an event to know when we're done
    static readonly ManualResetEvent finished = new(false);

    // Start out assuming success; we'll set this to 1 if we get a failed test
    static int result = 0;

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("usage: TestRunner <assembly> [typeName [typeName...]]");
            return 2;
        }

        var testAssembly = args[0];
        var typeNames = new List<string>();
        for (int idx = 1; idx < args.Length; ++idx)
            typeNames.Add(args[idx]);

        await using var runner = AssemblyRunner.WithoutAppDomain(testAssembly);
        runner.OnDiscoveryComplete = OnDiscoveryComplete;
        runner.OnExecutionComplete = OnExecutionComplete;
        runner.OnTestFailed = OnTestFailed;
        runner.OnTestPassed = OnTestPassed;
        runner.OnTestSkipped = OnTestSkipped;

        Console.WriteLine("Discovering...");

        var options = new AssemblyRunnerStartOptions { TypesToRun = [.. typeNames] };
        runner.Start(options);

        finished.WaitOne();
        finished.Dispose();

        return result;
    }

    static void OnDiscoveryComplete(DiscoveryCompleteInfo info)
    {
        lock (consoleLock)
            Console.WriteLine($"Running {info.TestCasesToRun} of {info.TestCasesDiscovered} tests...");
    }

    static void OnExecutionComplete(ExecutionCompleteInfo info)
    {
        lock (consoleLock)
            Console.WriteLine($"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({info.TestsFailed} failed, {info.TestsSkipped} skipped, {info.TestsNotRun} not run)");

        finished.Set();
    }

    static void OnTestFailed(TestFailedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("[FAIL] {0}: {1}", info.TestDisplayName, info.ExceptionMessage);
            if (info.ExceptionStackTrace != null)
                Console.WriteLine(info.ExceptionStackTrace);

            Console.ResetColor();
        }

        result = 1;
    }

    static void OnTestPassed(TestPassedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[PASS] {0}", info.TestDisplayName);
            Console.ResetColor();
        }
    }

    static void OnTestSkipped(TestSkippedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
            Console.ResetColor();
        }
    }
}
