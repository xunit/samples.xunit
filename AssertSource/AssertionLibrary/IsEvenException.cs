namespace Xunit.Sdk;

// You can define your own custom exception types if you wish, or you can throw anything
// from the Xunit.Sdk namespace and the stack trace will be automatically trimmed (because
// the exception resides in the Xunit.Sdk namespace).

public class IsEvenException : XunitException
{
    public IsEvenException(int value) : base($"The value {value} was not even.")
    { }
}
