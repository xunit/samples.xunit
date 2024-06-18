using Xunit;
using Xunit.Sdk;

namespace RetryFactExample
{
    /// <summary>
    /// Works just like [Fact] except that failures are retried (by default, 3 times).
    /// </summary>
    [XunitTestCaseDiscoverer("RetryFactExample.RetryTheoryDiscoverer", "RetryFactExample")]
    public class RetryTheoryAttribute : TheoryAttribute
    {
        /// <summary>
        /// Number of retries allowed for a failed test. If unset (or set less than 1), will
        /// default to 3 attempts.
        /// </summary>
        public int MaxRetries { get; set; }
    }
}
