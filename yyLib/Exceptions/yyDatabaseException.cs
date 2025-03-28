namespace yyLib
{
    /// <summary>
    /// The yyDatabaseException is thrown when there is a failure in accessing the database.
    /// This could occur due to connectivity issues, authentication failures, or other problems that prevent successful interaction with the database.
    /// It is used to indicate that the operation could not be completed due to issues with the database access layer, rather than the data itself.
    /// </summary>
    public class yyDatabaseException: yyException
    {
        public yyDatabaseException (string message) : base (message)
        {
        }

        public yyDatabaseException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
