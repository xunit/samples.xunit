using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;

public sealed class DatabaseFixture : IAsyncLifetime
{
    SqlConnection? connection;

    public SqlConnection Connection =>
        connection ?? throw new InvalidOperationException("The database connection has not been created");

    public int FooUserID { get; private set; }

    public async ValueTask InitializeAsync()
    {
        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DatabaseFixture.mdf;Initial Catalog=DatabaseFixture;Integrated Security=True";
        connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string sql = @"INSERT INTO Users (FirstName, LastName) VALUES ('foo', 'bar'); SELECT SCOPE_IDENTITY();";

        using var cmd = new SqlCommand(sql, connection);
        FooUserID = Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async ValueTask DisposeAsync()
    {
        string sql = @"DELETE FROM Users WHERE ID = @id;";

        using (var cmd = new SqlCommand(sql, Connection))
        {
            cmd.Parameters.AddWithValue("@id", FooUserID);
            await cmd.ExecuteNonQueryAsync();
        }

        Connection.Close();
    }
}
