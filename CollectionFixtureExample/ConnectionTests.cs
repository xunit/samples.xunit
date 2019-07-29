using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

// Can't get these tests to run? Make sure you've installed LocalDB, a feature in the (free) SQL Server Express.
// Note that you don't need the "instance" server of SQL Express, just the LocalDB feature.

[Collection("DatabaseCollection")]
public class ConnectionTests 
{
    DatabaseFixture database;

    public ConnectionTests(DatabaseFixture data)
    {
        database = data;
    }

    [Fact]
    public void ConnectionIsEstablished()
    {
        Assert.NotNull(database.Connection);
    }
}

