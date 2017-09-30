using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Xunit;
using Xunit.Sdk;


/// <summary>
/// Represents an implementation of <see cref="DataAttribute"/> which uses an
/// instance of <see cref="IDataAdapter"/> to get the data for a <see cref="TheoryAttribute"/>
/// decorated test method.
/// </summary>
public abstract class DataAdapterDataAttribute : DataAttribute
{
    /// <summary>
    /// Gets the data adapter to be used to retrieve the test data.
    /// </summary>
    protected abstract IDataAdapter DataAdapter { get; }

    /// <inheritdoc/>
    public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
    {
        DataSet dataSet = new DataSet();
        IDataAdapter adapter = DataAdapter;

        try
        {
            adapter.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
                yield return ConvertParameters(row.ItemArray);
        }
        finally
        {
            IDisposable disposable = adapter as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }

    object[] ConvertParameters(object[] values)
    {
        object[] result = new object[values.Length];

        for (int idx = 0; idx < values.Length; idx++)
            result[idx] = ConvertParameter(values[idx]);

        return result;
    }

    /// <summary>
    /// Converts a parameter to its destination parameter type, if necessary.
    /// </summary>
    /// <param name="parameter">The parameter value</param>
    /// <returns>The converted parameter value</returns>
    protected virtual object ConvertParameter(object parameter)
    {
        if (parameter is DBNull)
            return null;

        return parameter;
    }
}
