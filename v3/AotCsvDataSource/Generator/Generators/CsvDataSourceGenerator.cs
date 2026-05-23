using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Xunit.Generators;

namespace AotCsvDataSource.Generator;

[Generator(LanguageNames.CSharp)]
public class CsvDataSourceGenerator() :
    DataAttributeGenerator("AotCsvDataSource.CsvDataAttribute")
{
    protected override void ProcessAttribute(
        SemanticModel semanticModel,
        INamedTypeSymbol testClass,
        IMethodSymbol testMethod,
        AttributeData attribute,
        DataAttributeGeneratorResult result,
        CancellationToken cancellationToken)
    {
        if (attribute.ConstructorArguments.Length < 1
                || attribute.ConstructorArguments[0].Kind != TypedConstantKind.Primitive
                || attribute.ConstructorArguments[0].Value is not string fileName)
            return;

        var dataAttributeRegistration = DataAttributeRegistration.TryGenerate<DataAttributeRegistration>(semanticModel, testClass, testMethod, attribute);
        if (dataAttributeRegistration is null)
            return;

        var builder = new StringBuilder();

        builder.Append($$"""
            async disposalTracker => {
                var attr = {{dataAttributeRegistration}};
                var result = new global::System.Collections.Generic.List<global::Xunit.ITheoryDataRow>();
                var filePath = global::System.IO.Path.Combine(global::System.AppContext.BaseDirectory, {{fileName.ToCSharp()}});
                var csvReader = global::nietras.SeparatedValues.SepReaderExtensions.FromFile(new global::nietras.SeparatedValues.SepReaderOptions { Unescape = true }, filePath);
                foreach (var csvRow in csvReader)
                {
                    var invalidRows = new global::System.Collections.Generic.List<(string Type, string Name, string Value)>();

            """);

        var parameterNames = new List<string>();

        for (var idx = 0; idx < testMethod.Parameters.Length; ++idx)
        {
            var parameter = testMethod.Parameters[idx];
            var parameterName = $"value{idx}";
            parameterNames.Add(parameterName);

            builder.Append($$"""
                        if (!csvRow[{{idx}}].TryParse<{{parameter.Type.ToCSharp()}}>(out var {{parameterName}}))
                            invalidRows.Add(({{parameter.Type.ToDisplayString().ToCSharp()}}, {{parameter.Name.ToCSharp()}}, csvRow[{{idx}}].ToString()));

                """);
        }

        builder.Append($$"""
                    if (invalidRows.Count != 0)
                            throw new global::Xunit.Sdk.TestPipelineException(
                                string.Format(
                                    global::System.Globalization.CultureInfo.CurrentCulture,
                                    "CSV data row {0} from 'SampleData.csv' had one or more invalid theory data values: {1}",
                                    csvRow.RowIndex,
                                    string.Join(", ", global::System.Linq.Enumerable.Select(invalidRows, a => $"{a.Type} {a.Name} ({a.Value})"))
                                )
                            );
                    result.Add(attr.CreateDataRow(new object[] { {{string.Join(", ", parameterNames)}} }));
                }
                return result;
            }
            """);

        result.Factories.Add(new(builder.ToString(), disableDiscoveryEnumeration: false));
    }
}
