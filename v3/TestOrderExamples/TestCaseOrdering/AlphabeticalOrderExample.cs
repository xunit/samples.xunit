using TestOrderExamples.TestCaseOrdering;
using Xunit;

[TestCaseOrderer(typeof(AlphabeticalOrderer))]
public class AlphabeticalOrderExample
{
    static bool Test1Called;
    static bool Test2Called;
    static bool Test3Called;

    [Fact]
    public void Test3()
    {
        Test3Called = true;

        Assert.True(Test1Called);
        Assert.True(Test2Called);
    }

    [Fact]
    public void Test1()
    {
        Test1Called = true;

        Assert.False(Test2Called);
        Assert.False(Test3Called);
    }

    [Fact]
    public void Test2()
    {
        Test2Called = true;

        Assert.True(Test1Called);
        Assert.False(Test3Called);
    }
}
