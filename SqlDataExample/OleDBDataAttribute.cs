using System;
using System.Data;
using System.Data.OleDb;

/// <summary>
/// Provides a data source for a data theory, with the data coming from an OLEDB connection.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class OleDbDataAttribute : DataAdapterDataAttribute
{
    readonly string connectionString;
    readonly string selectStatement;

    /// <summary>
    /// Creates a new instance of <see cref="OleDbDataAttribute"/>.
    /// </summary>
    /// <param name="connectionString">The OLEDB connection string to the data</param>
    /// <param name="selectStatement">The SELECT statement used to return the data for the theory</param>
    public OleDbDataAttribute(string connectionString, string selectStatement)
    {
        this.connectionString = connectionString;
        this.selectStatement = selectStatement;
    }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    public string ConnectionString
    {
        get { return connectionString; }
    }

    /// <summary>
    /// Gets the select statement.
    /// </summary>
    public string SelectStatement
    {
        get { return selectStatement; }
    }

    /// <inheritdoc/>
    protected override IDataAdapter DataAdapter
    {
        get { return new OleDbDataAdapter(selectStatement, connectionString); }
    }
}
