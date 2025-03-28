namespace yyLib
{
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
