using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace SqlDataExample;

/// <summary>
/// Represents an implementation of <see cref="DataAttribute"/> which uses an
/// instance of <see cref="IDataReader"/> to get the data for a <see cref="TheoryAttribute"/>
/// decorated test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class DataReaderDataAttribute : DataAttribute
{
    /// <summary>
    /// Returns <c>true</c> if the data attribute wants to enumerate data during discovery. By
    /// default, this attribute assumes that data enumeration is too expensive to do during
    /// discovery, and therefore defaults to <c>false</c>.
    /// </summary>
    public bool EnableDiscoveryEnumeration { get; set; }

    TheoryDataRow ConvertParameters(object?[] values)
    {
        var result = new object?[values.Length];

        for (int idx = 0; idx < values.Length; idx++)
            result[idx] = ConvertParameter(values[idx]);

        return new TheoryDataRow(result);
    }

    /// <summary>
    /// Converts a parameter to its destination parameter type, if necessary.
    /// </summary>
    /// <param name="parameter">The parameter value</param>
    /// <returns>The converted parameter value</returns>
    protected virtual object? ConvertParameter(object? parameter)
    {
        if (parameter is DBNull)
            return null;

        return parameter;
    }

    /// <inheritdoc/>
    public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        var result = new List<ITheoryDataRow>();
        var reader = await GetDataReader(disposalTracker);
        disposalTracker.Add(reader);

        while (reader.Read())
        {
            var fields = new object[reader.FieldCount];
            reader.GetValues(fields);

            disposalTracker.AddRange(fields);
            result.Add(ConvertParameters(fields));
        }

        return result;
    }

    /// <summary>
    /// Gets the data reader to be used to retrieve the test data.
    /// </summary>
    protected abstract ValueTask<IDataReader> GetDataReader(DisposalTracker disposalTracker);

    /// <inheritdoc/>
    public override bool SupportsDiscoveryEnumeration() =>
        EnableDiscoveryEnumeration;
}
