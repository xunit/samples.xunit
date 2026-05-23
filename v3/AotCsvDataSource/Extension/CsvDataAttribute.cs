using System;
using Xunit.v3;

namespace AotCsvDataSource;

/// <summary>
/// Apply this attribute to your test method to retrieve data from a CSV file.
/// </summary>
/// <param name="fileName">The file name (assumed to be a relative path to <see cref="AppContext.BaseDirectory"/>)</param>
public class CsvDataAttribute(string fileName) : DataAttribute
{
    // The property is here to satisfy the "rules" for attributes, even though the
    // generator pulls the file name from the constructor.
    public string FileName { get; } = fileName;
}
