using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace STAExamples
{
    public class STAFactDiscoverer : IXunitTestCaseDiscoverer
    {
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var xUnitTestCase = new XunitTestCase(discoveryOptions.MethodDisplayOrDefault(), testMethod, new object[] { });
            return new IXunitTestCase[] { new STATestCase(xUnitTestCase) };
        }
    }
}
