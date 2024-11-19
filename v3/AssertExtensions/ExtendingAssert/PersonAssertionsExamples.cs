using Xunit;

// This is an example of writing your own assertion that lives under Assert, by importing the assertion
// library as source rather than as binary, so that you can write more assertions on the Assert
// partial class.
public class PersonAssertionsExamples
{
    [Fact]
    public void EnsureBradness()
    {
        var person1 = new Person("Brad", "Wilson");
        var person2 = new Person("Brad", "Pitt");

        Assert.IsBrad(person1);
        Assert.IsBrad(person2);
    }
}
