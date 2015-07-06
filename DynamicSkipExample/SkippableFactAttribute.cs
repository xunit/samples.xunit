using Xunit;
using Xunit.Sdk;

namespace DynamicSkipExample
{
    [XunitTestCaseDiscoverer("DynamicSkipExample.XunitExtensions.SkippableFactDiscoverer", "DynamicSkipExample")]
    public class SkippableFactAttribute : FactAttribute { }
}
