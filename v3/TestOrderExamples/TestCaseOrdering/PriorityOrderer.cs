using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace TestOrderExamples.TestCaseOrdering;

public class PriorityOrderer : ITestMethodOrderer
{
    static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var result))
            return result;

        result = new TValue();
        dictionary[key] = result;

        return result;
    }

    public IReadOnlyCollection<TTestMethod?> OrderTestMethods<TTestMethod>(IReadOnlyCollection<TTestMethod?> testMethods)
        where TTestMethod : notnull, ITestMethod
    {
        var result = new List<TTestMethod?>();
        var sortedMethods = new SortedDictionary<int, List<IXunitTestMethod?>>();

        foreach (IXunitTestMethod? testMethod in testMethods)
        {
            var priority = 0;
            if (testMethod?.Method.GetCustomAttributes<TestPriorityAttribute>().FirstOrDefault() is { } attr)
                priority = attr.Priority;

            GetOrCreate(sortedMethods, priority).Add(testMethod);
        }

        foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
        {
            list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x?.Method.Name, y?.Method.Name));
            for (var i = 0; i < list.Count; i++)
                if (list[i] is TTestMethod tTestMethod)
                    result.Add(tTestMethod);
        }

        return result;
    }
}
