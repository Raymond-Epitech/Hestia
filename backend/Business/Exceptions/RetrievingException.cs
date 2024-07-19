namespace Business.Exceptions;

[Serializable]
public class RetreivingException : Exception
{
    public RetreivingException()
    { }

    public RetreivingException(string message)
    : base(message)
    { }

    public RetreivingException(string message, Exception innerException)
        : base(message, innerException)
    { }
}