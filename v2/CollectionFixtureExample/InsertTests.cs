using System;
using System.Data.SqlClient;
using Xunit;

// Can't get these tests to run? Make sure you've installed LocalDB, a feature in the (free) SQL Server Express.
// Note that you don't need the "instance" server of SQL Express, just the LocalDB feature.

[Collection("DatabaseCollection")]
public class InsertTests
{
    DatabaseFixture database;

    public InsertTests(DatabaseFixture fixture)
    {
        database = fixture;
    }

    [Fact]
    public void FooUserWasInserted()
    {
        string sql = "SELECT COUNT(*) FROM Users WHERE ID = @id;";

        using (SqlCommand cmd = new SqlCommand(sql, database.Connection))
        {
            cmd.Parameters.AddWithValue("@id", database.FooUserID);

            int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

            Assert.Equal(1, rowCount);
        }
    }
}
