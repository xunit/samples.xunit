using System;
using System.Configuration;
using System.Data.SqlClient;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DatabaseFixture"].ConnectionString;
        Connection = new SqlConnection(connectionString);
        Connection.Open();

        string sql = @"INSERT INTO Users VALUES ('foo', 'bar'); SELECT SCOPE_IDENTITY();";

        using (SqlCommand cmd = new SqlCommand(sql, Connection))
            FooUserID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public SqlConnection Connection { get; }

    public int FooUserID { get; }

    public void Dispose()
    {
        string sql = @"DELETE FROM Users WHERE ID = @id;";

        using (SqlCommand cmd = new SqlCommand(sql, Connection))
        {
            cmd.Parameters.AddWithValue("@id", FooUserID);
            cmd.ExecuteNonQuery();
        }

        Connection.Close();
    }
}
