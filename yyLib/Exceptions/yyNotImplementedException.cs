namespace yyLib
{
    /// <summary>
    /// The yyNotImplementedException is thrown when a requested method or operation has not been implemented.
    /// This exception is used to indicate that the functionality is planned but not yet available,
    /// serving as a placeholder for future development.
    /// </summary>
    public class yyNotImplementedException: yyException
    {
        public yyNotImplementedException (string message) : base (message)
        {
        }

        public yyNotImplementedException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
