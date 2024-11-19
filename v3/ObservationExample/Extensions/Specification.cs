namespace ObservationExample;

public abstract class Specification
{
    protected virtual void Because() { }

    protected virtual void DestroyContext() { }

    protected virtual void EstablishContext() { }

    internal void OnFinish()
    {
        DestroyContext();
    }

    internal void OnStart()
    {
        EstablishContext();
        Because();
    }
}
