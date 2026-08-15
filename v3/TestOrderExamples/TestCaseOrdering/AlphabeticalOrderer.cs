using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using Xunit.v3;

namespace TestOrderExamples.TestCaseOrdering;

public class AlphabeticalOrderer : ITestMethodOrderer
{
    public IReadOnlyCollection<TTestMethod?> OrderTestMethods<TTestMethod>(IReadOnlyCollection<TTestMethod?> testMethods)
        where TTestMethod : notnull, ITestMethod
    {
        var result = testMethods.Cast<IXunitTestMethod>().ToList();
        result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Method.Name, y.Method.Name));
        return [.. result.Cast<TTestMethod>()];
    }
}
