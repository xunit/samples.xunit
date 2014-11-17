using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace STAExamples
{
    public class STAFactDiscoverer : IXunitTestCaseDiscoverer
    {
        public IEnumerable<IXunitTestCase> Discover(ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield return new STATestCase(new XunitTestCase(testMethod));
        }
    }
}
