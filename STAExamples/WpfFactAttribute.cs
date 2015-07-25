using System;
using Xunit;
using Xunit.Sdk;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("STAExamples.WpfFactDiscoverer", "STAExamples")]
public class WpfFactAttribute : FactAttribute { }
