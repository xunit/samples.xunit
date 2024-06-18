using System;
using Xunit;
using Xunit.Sdk;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("STAExamples.WpfTheoryDiscoverer", "STAExamples")]
public class WpfTheoryAttribute : TheoryAttribute { }
