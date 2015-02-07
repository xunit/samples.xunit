using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace RetryFactExample
{
    public class RetryFactDiscoverer : IXunitTestCaseDiscoverer
    {
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var maxRetries = factAttribute.GetNamedArgument<int>("MaxRetries");
            if (maxRetries < 1)
                maxRetries = 3;

            yield return new RetryTestCase(discoveryOptions.MethodDisplayOrDefault(), testMethod, maxRetries);
        }
    }
}
