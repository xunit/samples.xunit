using AotTraitExtensibility;
using Xunit;

// If you run your test assembly with a `-list full` command line, then it will show you all your
// test cases with their categories, which for this will look something like:
//
// - Display name: "Samples.ExampleFact"
//   Test method:  Samples.ExampleFact
//   ID:           [...test case ID...]
//   Traits:
//     "Category": ["Assembly-level", "Class-level", "Collection-level", "Method-level"]
//     "Categorized": ["true"]
//
// You can then use traits as a filter when running tests, with `-trait`, `-trait-`, or `-filter`.

[assembly: Category("Assembly-level")]

[Category("Class-level")]
[Collection("Sample collection")]
public class Samples
{
    [Fact, Category("Method-level")]
    public void ExampleFact() =>
        Assert.True(true);
}

[Category("Collection-level")]
[CollectionDefinition("Sample collection")]
public class SampleCollection
{ }
