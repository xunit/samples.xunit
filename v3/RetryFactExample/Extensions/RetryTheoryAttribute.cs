using Xunit;
using Xunit.v3;

namespace RetryFactExample;

/// <summary>
/// Works just like [Theory] except that failures are retried (by default, 3 times).
/// </summary>
[XunitTestCaseDiscoverer(typeof(RetryTheoryDiscoverer))]
public class RetryTheoryAttribute : TheoryAttribute
{
    public int MaxRetries { get; set; } = 3;
}
