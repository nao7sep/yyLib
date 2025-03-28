namespace yyLib
{
    public class yyConcurrencyException: yyException
    {
        public yyConcurrencyException (string message) : base (message)
        {
        }

        public yyConcurrencyException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
