using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Sdk;

namespace SqlDataExample;

/// <summary>
/// Provides a data source for a data theory, with the data coming a Microsoft SQL Server.
/// </summary>
public class SqlServerDataAttribute : DataReaderDataAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="SqlServerDataAttribute"/>, using a trusted connection.
    /// </summary>
    /// <param name="serverName">The server name of the Microsoft SQL Server</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="selectStatement">The SQL SELECT statement to return the data for the data theory</param>
    public SqlServerDataAttribute(string serverName, string databaseName, string selectStatement)
        : this(GetConnectionStringWithTrust(serverName, databaseName), selectStatement)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SqlServerDataAttribute"/>, using the provided username and password.
    /// </summary>
    /// <param name="serverName">The server name of the Microsoft SQL Server</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="userName">The username for the server</param>
    /// <param name="password">The password for the server</param>
    /// <param name="selectStatement">The SQL SELECT statement to return the data for the data theory</param>
    public SqlServerDataAttribute(string serverName, string databaseName, string userName, string password, string selectStatement)
        : this(GetConnectionStringForUser(serverName, databaseName, userName, password), selectStatement)
    { }

    SqlServerDataAttribute(string connectionString, string selectStatement)
    {
        ConnectionString = connectionString;
        SelectStatement = selectStatement;
    }

    public string ConnectionString { get; }

    public string SelectStatement { get; }

    protected static string GetConnectionStringForUser(string serverName, string databaseName, string userName, string password) =>
        string.Format(
            CultureInfo.InvariantCulture,
            "Data Source={0}; Initial Catalog={1}; User ID={2}; Password={3};",
            serverName,
            databaseName,
            userName,
            password
        );

    protected static string GetConnectionStringWithTrust(string serverName, string databaseName) =>
        string.Format(
            CultureInfo.InvariantCulture,
            "Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI;",
            serverName,
            databaseName
        );

    protected override async ValueTask<IDataReader> GetDataReader(DisposalTracker disposalTracker)
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = SelectStatement;

        disposalTracker.Add(connection);
        disposalTracker.Add(command);
        return await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }
}
