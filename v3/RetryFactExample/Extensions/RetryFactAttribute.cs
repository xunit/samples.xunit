using System.Runtime.CompilerServices;
using Xunit;
using Xunit.v3;

namespace RetryFactExample;

/// <summary>
/// Works just like [Fact] except that failures are retried (by default, 3 times).
/// </summary>
[XunitTestCaseDiscoverer(typeof(RetryFactDiscoverer))]
public class RetryFactAttribute(
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int sourceLineNumber = -1) :
        FactAttribute(sourceFilePath, sourceLineNumber)
{
    public int MaxRetries { get; set; } = 3;
}
