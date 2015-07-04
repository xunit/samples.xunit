using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace PartialTrustExample.XunitExtensions
{
    public class PartialTrustTestCaseRunner : XunitTestCaseRunner
    {
        public PartialTrustTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, object[] testMethodArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource) { }

        protected override Task<RunSummary> RunTestAsync()
        {
            var test = new XunitTest(TestCase, DisplayName);
            return new PartialTrustTestRunner(test, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();
        }
    }
}
