using System;
using System.Collections.Generic;
using Xunit;

public class EqualExamples
{
    [Fact]
    public void EqualStringIgnoreCase()
    {
        string expected = "TestString";
        string actual = "teststring";

        Assert.False(actual == expected);
        Assert.NotEqual(expected, actual);
        Assert.Equal(expected, actual, StringComparer.CurrentCultureIgnoreCase);
    }

    class DateComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y) =>
            x.Date == y.Date;

        public int GetHashCode(DateTime obj) =>
            obj.GetHashCode();
    }

    [Fact]
    public void DateShouldBeEqualEvenThoughTimesAreDifferent()
    {
        var firstTime = DateTime.Now.Date;
        var later = firstTime.AddMinutes(90);

        Assert.NotEqual(firstTime, later);
        Assert.Equal(firstTime, later, new DateComparer());
    }
}
