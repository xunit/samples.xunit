#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

using AotCsvDataSource;
using Xunit;

// This sample relies on Sep to do the CSV parsing: https://github.com/nietras/Sep
//
// The extension doesn't consume it directly, but it does bring a reference that is transient
// so that the generated code can use it to parse.
//
// No effort is made to ensure that the parameter types are compatible with Sep's parsing, so
// generated code could end up with a compiler failure when types are incompatible.
//
// Data pre-enumeration is supported, so running the test app with `-list full` should result
// in output similar to this:
//
//   - Display name: "Samples.TestMethod(x: 1, y: \"Hello, World\", z: 21.12)"
//     Test method:  Samples.TestMethod
//     ID:           [...test case ID...]
//   - Display name: "Samples.TestMethod(x: 2, y: \"Everything is fine\", z: 2600)"
//     Test method:  Samples.TestMethod
//     ID:           [...test case ID...]

public static class Samples
{
    [Theory]
    [CsvData("SampleData.csv")]
    public static void TestMethod(int x, string y, decimal z)
    {
        Assert.Equal(1, x);
    }
}
