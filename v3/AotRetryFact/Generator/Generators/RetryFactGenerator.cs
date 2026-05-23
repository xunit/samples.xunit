using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit.Generators;

namespace AotRetryFact.Generator;

[TestMethodGenerator("AotRetryFact.RetryFactAttribute")]
public class RetryFactGenerator : ITestMethodGenerator
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

        var details = new FactMethodDetails(semanticModel, testClass, testMethodSyntax, testMethod, attribute);
        if (!details.Process())
            return null;

        var maxRetries = 3;
        var maxRetriesArgument = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == Names.RetryFactAttribute.MaxRetries);
        if (maxRetriesArgument.Value.Kind == TypedConstantKind.Primitive && maxRetriesArgument.Value.Value is int maxRetriesValue)
            maxRetries = maxRetriesValue;

        var initValues = new List<string>
        {
            $"MaxRetries = {maxRetries}",
            $"MethodInvoker = {details.MethodInvoker}"
        };

        if (details.DisplayName is not null)
            initValues.Add($"DisplayName = {details.DisplayName.ToCSharp()}");
        if (details.Explicit)
            initValues.Add("Explicit = true");
        if (details.SkipExceptions.Count != 0)
            initValues.Add($"SkipExceptions = new global::System.Type[] {{ {string.Join(", ", details.SkipExceptions.Select(e => $"typeof({e})"))} }}");
        if (details.SkipReason is not null)
            initValues.Add($"SkipReason = {details.SkipReason.ToCSharp()}");
        if (details.SkipUnless is not null)
            initValues.Add($"SkipUnless = () => {(details.SkipType ?? testClass).ToCSharp()}.{details.SkipUnless}");
        if (details.SkipWhen is not null)
            initValues.Add($"SkipWhen = () => {(details.SkipType ?? testClass).ToCSharp()}.{details.SkipWhen}");
        if (details.Timeout is not 0)
            initValues.Add($"Timeout = {details.Timeout}");

        return CodeGenTestMethodRegistration.FromTestMethodDetails(details, $"new global::AotRetryFact.RetryFactTestCaseFactory() {{ {string.Join(", ", initValues)} }}");
    }
}
