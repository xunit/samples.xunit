using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xunit;

// Because we are bringing in the assertion library as source code, we can extend the partial
// class "Assert" to add our own custom assertions.

partial class Assert
{
    /// <summary>
    /// Verifies that the given value is even.
    /// </summary>
    /// <param name="value">The value to test for even-ness.</param>
    /// <exception cref="IsEvenException">Thrown if the value was not even.</exception>
    public static void IsEven(int value)
    {
        if (value % 2 != 0)
            throw IsEvenException.ForNonEvenValue(value);
    }

    /// <summary>
    /// Verifies that the given value is even.
    /// </summary>
    /// <param name="value">The value to test for even-ness.</param>
    /// <exception cref="IsEvenException">Thrown if the value was not even.</exception>
    public static ValueTask IsEvenAsync(int value)
    {
        // This example is to show usage of Assert.AllAsync, which is not normally available in v2.
        IsEven(value);
        return default;
    }
}
