using System;
using System.Runtime.CompilerServices;

namespace AotRetryFact;

// The properties added here are copied from FactAttribute, since that class is now sealed. The
// source generator utility will read these properties by name, so type isn't important. This also
// allows the user to craft an attribute with the exact API shape, removing any attributes that
// are no longer deemed relevant.

/// <summary>
/// Works just like [Fact] except that failures are retried (by default, 3 times).
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RetryFactAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RetryFactAttribute"/> class.
    /// </summary>
    public RetryFactAttribute(
#pragma warning disable IDE0060 // These parameter values are used by the source generator
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1)
#pragma warning restore IDE0060
    { }

    /// <summary>
    /// Gets the name of the test to be used when the test is skipped. When <see langword="null"/>
    /// is returned, will cause a default display name to be used.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets a flag which indicates whether the test should only be run explicitly.
    /// An explicit test is skipped by default unless explicit tests are requested
    /// to be run.
    /// </summary>
    public bool Explicit { get; set; }

    /// <summary>
    /// The maximum number of times to retry the test before failure. Defaults to <c>3</c>.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets the skip reason for the test. When <see langword="null"/> is returned, the test is
    /// not skipped.
    /// </summary>
    /// <remarks>
    /// Skipping is conditional based on whether <see cref="SkipWhen"/> or <see cref="SkipUnless"/>
    /// is set.
    /// </remarks>
    public string? Skip { get; set; }

    /// <summary>
    /// Gets exceptions that, when thrown, will cause the test to be skipped rather than failed.
    /// </summary>
    /// <remarks>
    /// The skip reason will be the exception's message.
    /// </remarks>
    public Type[]? SkipExceptions { get; set; }

    /// <summary>
    /// Gets the type to retrieve <see cref="SkipUnless"/> or <see cref="SkipWhen"/> from. If not set,
    /// then the property will be retrieved from the unit test class.
    /// </summary>
    public Type? SkipType { get; set; }

    /// <summary>
    /// Gets the name of a public static property on the test class which returns <see cref="bool"/>
    /// to indicate whether the test should be skipped (<see langword="false"/>) or not (<see langword="true"/>).
    /// </summary>
    /// <remarks>
    /// This property cannot be set if <see cref="SkipWhen"/> is set. Setting both will
    /// result in a failed test.<br />
    /// <br />
    /// To ensure compile-time safety and easier refactoring, use the <see langword="nameof"/> operator,
    /// e.g., <c>SkipUnless = nameof(IsConditionMet)</c>.
    /// </remarks>
    public string? SkipUnless { get; set; }

    /// <summary>
    /// Gets the name of a public static property on the test class which returns <see cref="bool"/>
    /// to indicate whether the test should be skipped (<see langword="true"/>) or not (<see langword="false"/>).
    /// </summary>
    /// <remarks>
    /// This property cannot be set if <see cref="SkipUnless"/> is set. Setting both will
    /// result in a failed test.<br />
    /// <br />
    /// To ensure compile-time safety and easier refactoring, use the <see langword="nameof"/> operator,
    /// e.g., <c>SkipWhen = nameof(IsConditionMet)</c>.
    /// </remarks>
    public string? SkipWhen { get; set; }

    /// <summary>
    /// Gets the timeout for test (in milliseconds). When <c>0</c> is returned, the test
    /// will not have a timeout.
    /// </summary>
    /// <remarks>
    /// WARNING: Using this with <see cref="ParallelAlgorithm.Aggressive"/> will result
    /// in undefined behavior. Test timing and timeouts are only reliable when using
    /// <see cref="ParallelAlgorithm.Conservative"/> (or when parallelization is disabled
    /// completely).
    /// </remarks>
    public int Timeout { get; set; }
}
