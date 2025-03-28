namespace yyLib
{
    /// <summary>
    /// The yyNullReferenceException is thrown when a reference retrieved from a method or operation is unexpectedly null.
    /// This exception is useful for ensuring that critical references are not null, which can prevent runtime errors and ensure the stability of the application.
    /// A common usage pattern is to use it with the null-coalescing operator (??) to throw an exception if a reference is null:
    /// exampleObject ?? throw new yyNullReferenceException("exampleObject should not be null");
    /// </summary>
    public class yyNullReferenceException: yyException
    {
        public yyNullReferenceException (string message) : base (message)
        {
        }

        public yyNullReferenceException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
