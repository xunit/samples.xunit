using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit.Generators;

namespace AotRetryFact.Generator;

[TestMethodGenerator("AotRetryFact.RetryTheoryAttribute")]
public class RetryTheoryGenerator : ITestMethodGenerator
{
    static readonly HashSet<string> validReturnTypes = ["void", Types.System.Threading.Tasks.Task, Types.System.Threading.Tasks.ValueTask];

    public CodeGenTestMethodRegistration? GetTestMethodRegistration(
        SemanticModel semanticModel,
        INamedTypeSymbol testClass,
        MethodDeclarationSyntax testMethodSyntax,
        IMethodSymbol testMethod,
        AttributeData attribute)
    {
        if (!validReturnTypes.Contains(testMethod.ReturnType.ToCSharp(includeGlobal: false)))
            return null;

        if (testMethod.IsGenericMethod || testMethod.Parameters.Any(p => p.IsParams))
            return null;

        var details = new TheoryMethodDetails(semanticModel, testClass, testMethodSyntax, testMethod, attribute);
        if (!details.Process())
            return null;

        var maxRetries = 3;
        var maxRetriesArgument = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == Names.RetryFactAttribute.MaxRetries);
        if (maxRetriesArgument.Value.Kind == TypedConstantKind.Primitive && maxRetriesArgument.Value.Value is int maxRetriesValue)
            maxRetries = maxRetriesValue;

        var initValues = new List<string>
        {
            $"MaxRetries = {maxRetries}",
            $"MethodInvokerFactory = {details.MethodInvokerFactory}",
            $"ParameterNames = new string?[] {{ {string.Join(", ", details.ParameterNames.Select(p => p.ToCSharp()))} }}"
        };

        if (details.DisableDiscoveryEnumeration is not null)
            initValues.Add($"DisableDiscoveryEnumeration = {details.DisableDiscoveryEnumeration.ToCSharp()}");
        if (details.DisplayName is not null)
            initValues.Add($"DisplayName = {details.DisplayName.ToCSharp()}");
        if (details.Explicit)
            initValues.Add("Explicit = true");
        if (details.IncludeTestCaseIndex)
            initValues.Add("IncludeTestCaseIndex = true");
        if (details.ParameterDefaultValues is not null)
            initValues.Add($"ParameterDefaultValues = new string?[] {{ {string.Join(", ", details.ParameterDefaultValues.Select(p => p.ToCSharp()))} }}");
        if (details.SkipExceptions.Count != 0)
            initValues.Add($"SkipExceptions = new global::System.Type[] {{ {string.Join(", ", details.SkipExceptions.Select(e => $"typeof({e})"))} }}");
        if (details.SkipReason is not null)
            initValues.Add($"SkipReason = {details.SkipReason.ToCSharp()}");
        if (details.SkipTestWithoutData)
            initValues.Add("SkipTestWithoutData = true");
        if (details.SkipUnless is not null)
            initValues.Add($"SkipUnless = () => {(details.SkipType ?? testClass).ToCSharp()}.{details.SkipUnless}");
        if (details.SkipWhen is not null)
            initValues.Add($"SkipWhen = () => {(details.SkipType ?? testClass).ToCSharp()}.{details.SkipWhen}");
        if (details.Timeout is not 0)
            initValues.Add($"Timeout = {details.Timeout}");

        return CodeGenTestMethodRegistration.FromTestMethodDetails(details, $"new global::AotRetryFact.RetryTheoryTestCaseFactory() {{ {string.Join(", ", initValues)} }}");
    }
}
