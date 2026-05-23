using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Xunit.Generators;

namespace AotTraitExtensibility.Generator;

[Generator(LanguageNames.CSharp)]
public class CategoryAttributeGenerator() :
    TraitGenerator("AotTraitExtensibility.CategoryAttribute")
{
    protected override IEnumerable<(string name, string value)> GetTraitValues(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length < 1 || attribute.ConstructorArguments[0].Kind != TypedConstantKind.Primitive)
            yield break;

        if (attribute.ConstructorArguments[0].Value is not string categoryName)
            yield break;

        yield return ("Category", categoryName);
        yield return ("Categorized", "true");
    }
}
