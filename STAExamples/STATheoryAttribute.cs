using System;
using Xunit;
using Xunit.Sdk;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("STAExamples.STATheoryDiscoverer", "STAExamples")]
public class STATheoryAttribute : TheoryAttribute
{
}
