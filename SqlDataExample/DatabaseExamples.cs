using System;
using Xunit;

public class DatabaseExamples
{
    /// <summary>
    /// Connects to a SQL Server database called TestDatabase (You will need to pass in your own database name)
    /// </summary>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    [Theory]
    [SqlServerData("(local)", "TestDatabase", "select FirstName, LastName from Users")]
    public void SqlServerTests(string FirstName, string LastName)
    {
        Assert.Equal("Peter Beardsley", String.Format("{0} {1}", FirstName, LastName));
    }

    /// <summary>
    /// Connects to an OleDB datasource using a connectionstring
    /// </summary>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    [Theory]
    [OleDbData(@"Provider=SQLOLEDB;Server=(local);Database=TestDatabase;Trusted_Connection=yes;", "select FirstName, LastName from Users")]
    public void OleDbTests(string FirstName, string LastName)
    {
        Assert.Equal("Peter Beardsley", String.Format("{0} {1}", FirstName, LastName));
    }
}