namespace yyLib
{
    /// <summary>
    /// The yyNotSupportedException is thrown when a requested method or operation is not supported.
    /// This exception indicates that the functionality is not intended to be available,
    /// either due to design decisions or limitations of the current implementation.
    /// </summary>
    public class yyNotSupportedException: yyException
    {
        public yyNotSupportedException (string message) : base (message)
        {
        }

        public yyNotSupportedException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
