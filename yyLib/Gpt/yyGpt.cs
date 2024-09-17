namespace yyLib
{
    public static class yyGpt
    {
        /// <summary>
        /// Returns null if the .yyUserSecrets.json file doesnt exist or contain a corresponding value.
        /// </summary>
        public static string? DefaultApiKey { get; } = yyUserSecrets.Default.OpenAi?.ApiKey.WhiteSpaceToNull ();

        /// <summary>
        /// Returns null if the .yyUserSecrets.json file doesnt exist or contain a corresponding value.
        /// </summary>
        public static string? DefaultOrganization { get; } = yyUserSecrets.Default.OpenAi?.Organization.WhiteSpaceToNull ();

        /// <summary>
        /// Returns null if the .yyUserSecrets.json file doesnt exist or contain a corresponding value.
        /// </summary>
        public static string? DefaultProject { get; } = yyUserSecrets.Default.OpenAi?.Project.WhiteSpaceToNull ();

        // Unlike Python's "HTTPX", C#'s HttpClient takes only one timeout value, that is used for everything.
        // In Python, values for connect, read, write and pool may differ.
        // https://www.python-httpx.org/advanced/timeouts/

        // Also, there doesnt seem to be any way to set a value other than that of the Timeout property to methods of HttpClient upon calling them.
        // pyddle_openai.py contains DEFAULT_TIMEOUT, DEFAULT_RESPONSE_TIMEOUT and DEFAULT_CHUNK_TIMEOUT, where the values are different.
        // If we want yyGptChatClient and yyGptImagesClient to work similarly, we'd need to dynamically adjust the value of the Timeout property of HttpClient.

        // If no value is specified and nullable value remains null, the default value is 100 seconds.
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout

        /// <summary>
        /// Returns null if the .yyUserSecrets.json file doesnt exist or contain a corresponding value.
        /// </summary>
        public static int? DefaultTimeout { get; } = yyUserSecrets.Default.OpenAi?.Timeout;
    }
}
