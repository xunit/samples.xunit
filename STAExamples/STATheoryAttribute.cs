using System;
using Xunit;
using Xunit.Sdk;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("STATheoryDiscoverer", "GitHub.Tests")]
public class STATheoryAttribute : TheoryAttribute
{
}
