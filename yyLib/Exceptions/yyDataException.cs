namespace yyLib
{
    /// <summary>
    /// The yyDataException is thrown when there is an issue with the data itself.
    /// This could include scenarios where the data is invalid, inconsistent, or does not meet the expected format or constraints.
    /// It is used to signal problems that arise from the data being processed, rather than issues with the data source or retrieval process.
    /// </summary>
    public class yyDataException: yyException
    {
        public yyDataException (string message) : base (message)
        {
        }

        public yyDataException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
