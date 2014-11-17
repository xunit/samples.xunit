using System;
using Xunit;
using Xunit.Sdk;

namespace STAExamples
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("STAExamples.STAFactDiscoverer", "STAExamples")]
    public class STAFactAttribute : FactAttribute
    {
    }
}