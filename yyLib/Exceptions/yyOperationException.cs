namespace yyLib
{
    /// <summary>
    /// The yyOperationException is thrown when an operation is attempted in an incorrect context or sequence, indicating a logical error in the code.
    /// This exception is used to signal that the developer is attempting to perform an operation that is not valid at the current point in the program's execution,
    /// such as calling methods in an incorrect order or violating expected preconditions.
    /// </summary>
    public class yyOperationException: yyException
    {
        public yyOperationException (string message) : base (message)
        {
        }

        public yyOperationException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
