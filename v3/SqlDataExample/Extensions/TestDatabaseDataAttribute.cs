using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SqlDataExample;
using Xunit;

// We use a fixture to ensure the database is properly created & populated with test data,
// and we do that at the assembly level so all tests can share in that data.
[assembly: AssemblyFixture(typeof(TestDatabaseDataAttribute.Fixture))]

namespace SqlDataExample;

/// <summary>
/// This class inherits from <see cref="SqlServerDataAttribute"/> with connection information
/// for the test database that we expect to be available.
/// </summary>
public class TestDatabaseDataAttribute(string selectStatement) :
    SqlServerDataAttribute(ServerName, DatabaseName, selectStatement)
{
    /// <summary>
    /// The database name where our test data lives
    /// </summary>
    public const string DatabaseName = "SqlDataExampleDb";

    /// <summary>
    /// The SQL Server server name (assumes SQL Server Express 2019 LocalDB is installed)
    /// </summary>
    public const string ServerName = "(localdb)\\MSSQLLocalDB";

    internal static string GetConnectionString(string databaseName) =>
        GetConnectionStringWithTrust(ServerName, databaseName);

    internal class Fixture : IAsyncLifetime
    {
        public ValueTask DisposeAsync() =>
            default;

        public async ValueTask InitializeAsync()
        {
            var ct = TestContext.Current.CancellationToken;

            // Connect to master so we can manipulate databases
            using var masterConnection = new SqlConnection(GetConnectionString("master"));
            await masterConnection.OpenAsync(ct);

            TestContext.Current.SendDiagnosticMessage("Validating connection to {0}", ServerName);

            using var validationCommand = new SqlCommand("""
                SELECT *
                  FROM [INFORMATION_SCHEMA].[TABLES]
                 WHERE 1=0
                """, masterConnection);
            await validationCommand.ExecuteNonQueryAsync(ct);

            TestContext.Current.SendDiagnosticMessage("Ensuring database {0} exists", DatabaseName);

            using var createDbCommand = new SqlCommand($"""
                IF DB_ID('{DatabaseName}') IS NULL
                BEGIN
                    CREATE DATABASE [{DatabaseName}]
                END
                """, masterConnection);
            await createDbCommand.ExecuteNonQueryAsync(ct);

            // Connect to the target database so we can manipulate tables & data
            using var dbConnection = new SqlConnection(GetConnectionString(DatabaseName));
            await dbConnection.OpenAsync(ct);

            TestContext.Current.SendDiagnosticMessage("Dropping table Users if it exists");

            using var dropTableCommand = new SqlCommand("""
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                    DROP TABLE [dbo].[Users]
                """, dbConnection);
            await dropTableCommand.ExecuteNonQueryAsync(ct);

            TestContext.Current.SendDiagnosticMessage("Creating table Users");

            using var createTableCommand = new SqlCommand("""
                CREATE TABLE [dbo].[Users] (
                    [FirstName] [nvarchar](max) NOT NULL,
                    [LastName] [nvarchar](max) NOT NULL
                )
                """, dbConnection);
            await createTableCommand.ExecuteNonQueryAsync(ct);

            TestContext.Current.SendDiagnosticMessage("Adding test data for table Users");

            using var populateCommand = new SqlCommand("""
                INSERT INTO [dbo].[Users]
                    (FirstName, LastName)
                VALUES
                    (N'Brad', N'Wilson')
                """, dbConnection);
            await populateCommand.ExecuteNonQueryAsync(ct);
        }
    }
}
