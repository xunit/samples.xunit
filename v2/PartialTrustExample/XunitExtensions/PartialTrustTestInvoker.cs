using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PartialTrustExample.XunitExtensions
{
    public class PartialTrustTestInvoker : XunitTestInvoker
    {
        PartialTrustSandbox sandbox;

        public PartialTrustTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource) { }

        protected override Task AfterTestMethodInvokedAsync()
        {
            if (sandbox != null)
                sandbox.Dispose();

            return base.AfterTestMethodInvokedAsync();
        }

        protected override object CreateTestClass()
        {
            sandbox = new PartialTrustSandbox();

            try
            {
                return sandbox.CreateInstance(TestClass, ConstructorArguments);
            }
            catch (SerializationException)
            {
                sandbox.Dispose();
                sandbox = null;

                throw new InvalidOperationException(string.Format("Test class {0} must derive from MarshalByRefObject to use [PartialTrustFact]", TestClass.FullName));
            }
        }
    }
}
