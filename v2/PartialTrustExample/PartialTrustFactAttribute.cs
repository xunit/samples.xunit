using System;
using Xunit;
using Xunit.Sdk;

namespace PartialTrustExample
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("PartialTrustExample.XunitExtensions.PartialTrustFactDiscoverer", "PartialTrustExample")]
    public class PartialTrustFactAttribute : FactAttribute { }
}
