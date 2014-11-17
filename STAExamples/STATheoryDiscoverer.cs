using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace STAExamples
{
    public class STATheoryDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly TheoryDiscoverer theoryDiscoverer = new TheoryDiscoverer();

        public IEnumerable<IXunitTestCase> Discover(ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return theoryDiscoverer.Discover(testMethod, factAttribute)
                .Select(testCase => new STATestCase(testCase));
        }
    }
}
