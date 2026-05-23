using System.Runtime.CompilerServices;

namespace AotRetryFact;

// The properties added here are copied from TheoryAttribute, since that class is now sealed. The
// source generator utility will read these properties by name, so type isn't important. This also
// allows the user to craft an attribute with the exact API shape, removing any attributes that
// are no longer deemed relevant.

/// <summary>
/// Works just like [Theory] except that failures are retried (by default, 3 times).
/// </summary>
public class RetryTheoryAttribute : RetryFactAttribute
{
    public RetryTheoryAttribute(
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1) :
            base(sourceFilePath, sourceLineNumber)
    { }

    /// <summary>
    /// Gets a flag which indicates whether the test method wants to skip enumerating data during
    /// discovery. This will cause the theory to yield a single test case for all data, and the
    /// data discovery will be performed during test execution instead of discovery.
    /// </summary>
    public bool DisableDiscoveryEnumeration { get; set; }

    /// <summary>
    /// Gets a flag which indicates whether each test case generated from data sources
    /// (<see cref="InlineDataAttribute"/>, <see cref="MemberDataAttribute"/>, and
    /// <see cref="ClassDataAttribute"/>) should include an auto-incremented, zero-padded
    /// index in its display name.
    /// </summary>
    public bool IncludeTestCaseIndex { get; set; }

    /// <summary>
    /// Gets a flag which indicates whether the test should be skipped (rather than failed) for
    /// a lack of data.
    /// </summary>
    public bool SkipTestWithoutData { get; set; }
}
