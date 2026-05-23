using System;

namespace AotTraitExtensibility;

/// <summary>
/// Apply this attribute to your test method to specify a category.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CategoryAttribute(string category) : Attribute
{
    // The property is here to satisfy the "rules" for attributes, even though the
    // generator pulls the category name from the constructor.
    public string Category { get; } = category;
}
