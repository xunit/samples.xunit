using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace STAExamples
{
    /// <summary>
    /// Wraps test cases for FactAttribute and TheoryAttribute so the test case runs in the STA Thread
    /// </summary>
    [Serializable]
    [DebuggerDisplay(@"\{ class = {TestMethod.TestClass.Class.Name}, method = {TestMethod.Method.Name}, display = {DisplayName}, skip = {SkipReason} \}")]
    public class STATestCase : IXunitTestCase, ISerializable
    {
        private readonly IXunitTestCase testCase;

        public STATestCase(IXunitTestCase testCase)
        {
            this.testCase = testCase;
        }

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", error: true)]
        public STATestCase() { }

        /// <inheritdoc />
        protected STATestCase(SerializationInfo info, StreamingContext context)
        {
            testCase = (IXunitTestCase)info.GetValue("InnerTestCase", typeof(IXunitTestCase));
        }

        public IMethodInfo Method
        {
            get { return testCase.Method; }
        }

        public Task<RunSummary> RunAsync(IMessageBus messageBus, object[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var tcs = new TaskCompletionSource<RunSummary>();
            var thread = new Thread(() =>
            {
                try
                {
                    var testCaseTask = testCase.RunAsync(messageBus, constructorArguments, aggregator,
                        cancellationTokenSource);
                    tcs.SetResult(testCaseTask.Result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        public string DisplayName
        {
            get { return testCase.DisplayName; }
        }

        public string SkipReason
        {
            get { return testCase.SkipReason; }
        }

        public ISourceInformation SourceInformation
        {
            get { return testCase.SourceInformation; }
            set { testCase.SourceInformation = value; }
        }

        public ITestMethod TestMethod
        {
            get { return testCase.TestMethod; }
        }

        public object[] TestMethodArguments
        {
            get { return testCase.TestMethodArguments; }
        }

        public Dictionary<string, List<string>> Traits
        {
            get { return testCase.Traits; }
        }

        public string UniqueID
        {
            get { return testCase.UniqueID; }
        }

        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InnerTestCase", testCase);
        }
    }
}