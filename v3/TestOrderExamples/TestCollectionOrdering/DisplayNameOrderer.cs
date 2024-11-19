using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using Xunit.v3;

namespace TestOrderExamples.TestCollectionOrdering;

public class DisplayNameOrderer : ITestCollectionOrderer
{
    public IReadOnlyCollection<TTestCollection> OrderTestCollections<TTestCollection>(IReadOnlyCollection<TTestCollection> testCollections)
        where TTestCollection : ITestCollection =>
            [.. testCollections.OrderBy(collection => collection.TestCollectionDisplayName)];
}
