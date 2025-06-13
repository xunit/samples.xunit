using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.SimpleRunner;

namespace TestRunner;

// This builds for both .NET Framework and .NET.
//
// The .NET Framework version can run v1/v2/v3 .NET Framework tests, and v3 .NET tests.
// The .NET version can run v3 .NET Framework tests, and v3 .NET tests.
class Program
{
    // Start out assuming success; we'll set this to 1 if we get a failed test
    static int result = 0;

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("usage: TestRunner <assembly> [typeName [typeName...]]");
            return 2;
        }

        // The options class allows you to not only influence how the test execution happens,
        // but also to subscribe to notifications of events during discovery and execution.
        var options = new AssemblyRunnerOptions(args[0])
        {
            OnDiagnosticMessage = msg => Console.WriteLine(">> Diagnostic message: {0} <<", msg.Message),
            OnDiscoveryStarting = OnDiscoveryStarting,
            OnDiscoveryComplete = OnDiscoveryComplete,
            OnErrorMessage = OnErrorMessage,
            OnExecutionStarting = OnExecutionStarting,
            OnExecutionComplete = OnExecutionComplete,
            OnInternalDiagnosticMessage = msg => Console.WriteLine(">> Internal diagnostic message: {0} <<", msg.Message),
            OnTestFailed = OnTestFailed,
            OnTestNotRun = OnTestNotRun,
            OnTestPassed = OnTestPassed,
            OnTestSkipped = OnTestSkipped,
        };

        // You can add reports that will be generated when the run is complete, i.e.:
        //options.AddReport(ReportType.HTML, "results.html");

        // You can also filter the tests. The XunitFilters class contains complete filtering
        // functionality. This example allows users to pass class name on the command line:
        for (int idx = 1; idx < args.Length; ++idx)
            options.Filters.AddIncludedClassFilter(args[idx]);

        await using (var runner = new AssemblyRunner(options))
            await runner.Run();

        return result;
    }

    static void OnDiscoveryComplete(DiscoveryCompleteInfo info)
    {
        Console.WriteLine($">> Discovery complete ({info.TestCasesToRun} test cases to run) <<");
    }

    static void OnDiscoveryStarting()
    {
        Console.WriteLine(">> Discovery starting <<");
    }

    static void OnErrorMessage(ErrorMessageInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine("[ERROR] {0}", info.ErrorMessageType);
        WriteException(info.Exception);

        Console.ResetColor();
    }

    static void OnExecutionComplete(ExecutionCompleteInfo info)
    {
        var totalSummaries = new List<string>();

        if (info.TestsPassed > 0)
            totalSummaries.Add($"{info.TestsPassed} passed");
        if (info.TestsFailed > 0)
            totalSummaries.Add($"{info.TestsFailed} failed");
        if (info.TestsSkipped > 0)
            totalSummaries.Add($"{info.TestsSkipped} skipped");
        if (info.TestsNotRun > 0)
            totalSummaries.Add($"{info.TestsNotRun} not run");
        if (info.TotalErrors > 0)
            totalSummaries.Add($"{info.TotalErrors} error{(info.TotalErrors == 1 ? "" : "s")}");

        var summary = $"{info.TestsTotal} test{(info.TestsTotal == 1 ? "" : "s")} in {Math.Round(info.ExecutionTime, 3)}s";
        if (totalSummaries.Count != 0)
            summary += $" :: {string.Join(", ", totalSummaries)}";

        Console.WriteLine($">> Execution complete ({summary}) <<");

        if (info.TestsFailed > 0)
            result = 1;
        if (info.TotalErrors > 0)
            result = 2;
    }

    static void OnExecutionStarting(ExecutionStartingInfo info)
    {
        Console.WriteLine(">> Execution starting <<");
    }

    static void OnTestFailed(TestFailedInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine("[FAIL] {0}", info.TestDisplayName);
        WriteException(info.Exception);

        Console.ResetColor();

        result = 1;
    }

    static void OnTestNotRun(TestNotRunInfo info)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("[NOT RUN] {0}", info.TestDisplayName);
        Console.ResetColor();
    }

    static void OnTestPassed(TestPassedInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[PASS] {0}", info.TestDisplayName);
        Console.ResetColor();
    }

    static void OnTestSkipped(TestSkippedInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
        Console.ResetColor();
    }

    static void WriteException(ExceptionInfo exception, string indent = "   ")
    {
        var indentedNewLine = Environment.NewLine + indent;

        Console.WriteLine("{0}{1}: {2}", indent, exception.FullType, exception.Message.Replace(Environment.NewLine, indentedNewLine));
        if (exception.StackTrace is not null)
            Console.WriteLine("{0}{1}", indent, exception.StackTrace.Replace(Environment.NewLine, indentedNewLine));

        var newIndent = indent + "   ";
        for (var idx = 0; idx < exception.InnerExceptions.Count; ++idx)
        {
            Console.WriteLine("{0}----- Inner exception #{1} -----", newIndent, idx + 1);
            WriteException(exception.InnerExceptions[idx], newIndent);
        }
    }
}
