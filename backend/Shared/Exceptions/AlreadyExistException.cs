namespace Shared.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException()
        { }

        public AlreadyExistException(string message)
        : base(message)
        { }

        public AlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

}
