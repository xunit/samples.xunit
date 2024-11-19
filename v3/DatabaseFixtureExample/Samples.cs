using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;

// Can't get these tests to run? Make sure you've installed "SQL Server Express 2019 LocalDB" in the Visual Studio
// features list.

public class Samples(DatabaseFixture data) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public void ConnectionIsEstablished()
    {
        Assert.NotNull(data.Connection);
    }

    [Fact]
    public async ValueTask FooUserWasInserted()
    {
        string sql = "SELECT COUNT(*) FROM Users WHERE ID = @id;";

        using var cmd = new SqlCommand(sql, data.Connection);
        cmd.Parameters.AddWithValue("@id", data.FooUserID);

        int rowCount = Convert.ToInt32(await cmd.ExecuteScalarAsync(TestContext.Current.CancellationToken));

        Assert.Equal(1, rowCount);
    }
}
