using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace TestRunner
{
    class Program
    {
        // We use consoleLock because messages can arrive in parallel, so we want to make sure we get
        // consistent console output.
        static object consoleLock = new object();

        // Use an event to know when we're done
        static ManualResetEvent finished = new ManualResetEvent(false);

        // Start out assuming success; we'll set this to 1 if we get a failed test
        static int result = 0;

        static int Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 2)
            {
                Console.WriteLine("usage: TestRunner <assembly> [typeName]");
                return 2;
            }

            var testAssembly = args[0];
            var typeName = args.Length == 2 ? args[1] : null;

            using (var runner = AssemblyRunner.WithAppDomain(testAssembly))
            {
                runner.OnDiscoveryComplete = OnDiscoveryComplete;
                runner.OnExecutionComplete = OnExecutionComplete;
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestSkipped = OnTestSkipped;

                Console.WriteLine("Discovering...");
                runner.Start(typeName);

                finished.WaitOne();
                finished.Dispose();

                return result;
            }
        }

        static void OnDiscoveryComplete(int testCasesDiscovered, int testCasesToRun)
        {
            lock (consoleLock)
                Console.WriteLine($"Running {testCasesToRun} of {testCasesDiscovered} tests...");
        }

        static void OnExecutionComplete(int totalTests, int testsFailed, int testsSkipped, decimal executionTime)
        {
            lock (consoleLock)
                Console.WriteLine($"Finished: {totalTests} tests in {Math.Round(executionTime, 3)}s ({testsFailed} failed, {testsSkipped} skipped)");

            finished.Set();
        }

        static void OnTestFailed(string typeName,
                                 string methodName,
                                 Dictionary<string, List<string>> traits,
                                 string testDisplayName,
                                 string testCollectionDisplayName,
                                 decimal executionTime,
                                 string output,
                                 string exceptionType,
                                 string message,
                                 string stackTrace)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("[FAIL] {0}: {1}", testDisplayName, message);
                if (stackTrace != null)
                    Console.WriteLine(stackTrace);

                Console.ResetColor();
            }

            result = 1;
        }

        static void OnTestSkipped(string typeName,
                                  string methodName,
                                  Dictionary<string, List<string>> traits,
                                  string testDisplayName,
                                  string testCollectionDisplayName,
                                  string skipReason)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[SKIP] {0}: {1}", testDisplayName, skipReason);
                Console.ResetColor();
            }
        }
    }
}
