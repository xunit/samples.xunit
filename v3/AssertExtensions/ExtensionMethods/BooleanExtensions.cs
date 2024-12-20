namespace Xunit.Extensions.AssertExtensions;

/// <summary>
/// Extensions which provide assertions to classes derived from <see cref="bool"/>.
/// </summary>
public static class BooleanExtensions
{
    /// <summary>
    /// Verifies that the condition is false.
    /// </summary>
    /// <param name="condition">The condition to be tested</param>
    /// <exception cref="FalseException">Thrown if the condition is not false</exception>
    public static void ShouldBeFalse(this bool condition) =>
        Assert.False(condition);

    /// <summary>
    /// Verifies that the condition is false.
    /// </summary>
    /// <param name="condition">The condition to be tested</param>
    /// <param name="userMessage">The message to show when the condition is not false</param>
    /// <exception cref="FalseException">Thrown if the condition is not false</exception>
    public static void ShouldBeFalse(this bool condition, string userMessage) =>
        Assert.False(condition, userMessage);

    /// <summary>
    /// Verifies that an expression is true.
    /// </summary>
    /// <param name="condition">The condition to be inspected</param>
    /// <exception cref="TrueException">Thrown when the condition is false</exception>
    public static void ShouldBeTrue(this bool condition) =>
        Assert.True(condition);

    /// <summary>
    /// Verifies that an expression is true.
    /// </summary>
    /// <param name="condition">The condition to be inspected</param>
    /// <param name="userMessage">The message to be shown when the condition is false</param>
    /// <exception cref="TrueException">Thrown when the condition is false</exception>
    public static void ShouldBeTrue(this bool condition, string userMessage) =>
        Assert.True(condition, userMessage);
}
