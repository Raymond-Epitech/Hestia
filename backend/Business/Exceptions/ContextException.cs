namespace Business.Exceptions;

public class ContextException : Exception
{
    public ContextException()
    { }

    public ContextException(string message)
    : base(message)
    { }

    public ContextException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
