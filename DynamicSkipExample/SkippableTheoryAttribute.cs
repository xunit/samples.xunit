using Xunit;
using Xunit.Sdk;

namespace DynamicSkipExample
{
    [XunitTestCaseDiscoverer("DynamicSkipExample.XunitExtensions.SkippableTheoryDiscoverer", "DynamicSkipExample")]
    public class SkippableTheoryAttribute : TheoryAttribute { }
}
