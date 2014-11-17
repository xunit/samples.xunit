using System;
using Xunit;
using Xunit.Sdk;

namespace STAExamples
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("STAFactDiscoverer", "GitHub.Tests")]
    public class STAFactAttribute : FactAttribute
    {
    }
}