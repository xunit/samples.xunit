using System;
using System.Collections.Generic;
using Xunit.v3;

/// <summary>
/// Apply this attribute to your test method to specify a category.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
class CategoryAttribute(string category) : Attribute, ITraitAttribute
{
    // Note that one trait attribute can provide as many traits as it needs to; you're not limited
    // to just one trait from one attribute.
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        [
            new("Category", category),
            new("Categorized", "true"),
        ];
}
