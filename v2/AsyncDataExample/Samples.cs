using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AsyncDataExample;

public class Samples
{
    // Properties can't be async, so they're unlikely data sources, but just want to show
    // that it can be used if needed.
    public static Task<IEnumerable<object?[]>> AsyncProperty
    {
        get => Task.FromResult<IEnumerable<object?[]>>([[42, 21.12m, "Hello world!"]]);
    }

    // Methods seem like the most common source of async data. We put a Task.Yield in here
    // just so we can show the async machinery is doing its thing.
    public static async Task<TheoryData<int, decimal, string?>> AsyncMethod()
    {
        await Task.Yield();

        return new()
        {
            { 2600, 3.14m, "Yes" },
            { 0, 0m, null },
        };
    }

    [Theory]
    [AsyncMemberData(nameof(AsyncProperty))]
    [AsyncMemberData(nameof(AsyncMethod))]
    public void TestMethod(int _1, decimal _2, string? _3)
    {
        Assert.Fail("Just want to see failures so we can see all the data");
    }
}
