namespace yyLib
{
    /// <summary>
    /// The yyException serves as a base exception class and should not be casually used.
    /// It is too vague and does not provide specific context about the error.
    /// Developers are encouraged to use more specific exception classes that convey meaningful information about the nature of the error,
    /// which aids in debugging and handling exceptions appropriately.
    /// </summary>
    public class yyException: Exception
    {
        public yyException (string message) : base (message)
        {
        }

        public yyException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
