using System;
using Xunit;
using Xunit.Sdk;

namespace STAExamples
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("STAExamples.WpfFactDiscoverer", "STAExamples")]
    public class WpfFactAttribute : FactAttribute { }
}