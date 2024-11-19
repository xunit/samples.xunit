using SqlDataExample;
using Xunit;

// NOTE: The tests here presume that you have installed SQL Server Express 2019 LocalDB,
// which is a feature that's available in the Visual Studio 2022 installer.

public class SqlServerDataAttributeExample
{
    // The presence of the data here is dependent on TestDatabaseDataAttribute setting
    // up the database, tables, and sample data via its assembly-level fixture
    [Theory]
    [SqlServerData(
        TestDatabaseDataAttribute.ServerName,
        TestDatabaseDataAttribute.DatabaseName,
        "select FirstName, LastName from Users"
    )]
    public void OnlyUserIsBrad(string firstName, string lastName) =>
        Assert.Equal("Brad Wilson", $"{firstName} {lastName}");
}
