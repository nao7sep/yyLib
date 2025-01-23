using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace yyLib
{
    public static class yyGptUtility
    {
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString, yyGptChatResponse Response, string [] Messages)> GenerateMessagesAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request, cancellationToken).ConfigureAwait (false);

            string xJsonString = await xClient.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
            yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);

#pragma warning disable CS8602 // Disables warnings for dereferencing a possibly null reference.
#pragma warning disable CS8604 // Disables warnings for passing a possibly null reference as an argument.
#pragma warning disable CS8619 // Disables warnings for conversion of nullability in type annotations.
            // The following code assumes that 'Choices', 'Message', and 'Content' are all non-null, as they are validated before this section of code.
            string [] xMessages = xResponse.Choices.Select (x => x.Message.Content as string).ToArray (); // We know Content is a string.
#pragma warning restore CS8602
#pragma warning restore CS8604
#pragma warning restore CS8619

            return (xSendingResult.ResponseMessage.IsSuccessStatusCode, xSendingResult.RequestJsonString, xJsonString, xResponse, xMessages);
        }

        public static async Task <(bool IsSuccess, string RequestJsonString, string [] ResponseJsonStrings, yyGptChatResponse [] Responses, string [] Messages)> GenerateMessagesChunksAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, Func <int, string?, CancellationToken, Task> onChunkRetrievedAsync, CancellationToken cancellationToken = default)
        {
            if (request.Stream is null or false)
                throw new yyArgumentException ("The request must be streaming.");

            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request, cancellationToken).ConfigureAwait (false);

            if (xSendingResult.ResponseMessage.IsSuccessStatusCode)
            {
                List <string> xJsonStrings = [];
                List <yyGptChatResponse> xResponses = [];
                yyAutoExpandingList <StringBuilder> xBuilders = [];

                while (true)
                {
                    // Each line should look like "data: { ..." or "data: [DONE]".
                    string? xLine = await xClient.ReadLineAsync (cancellationToken).ConfigureAwait (false);

                    // If null is returned before "data: [DONE]", something is wrong.
                    // It is highly unlikely that all the data is received and only the last line is missing.

                    if (xLine == null)
                        throw new yyFormatException ("The stream ended unexpectedly.");

                    if (string.IsNullOrWhiteSpace  (xLine))
                        continue; // Continues for "data: [DONE]".

                    xJsonStrings.Add (xLine);

                    var xResponse = yyGptChatResponseParser.ParseChunk (xLine);

                    if (xResponse == yyGptChatResponse.Empty)
                        break; // "data: [DONE]" is detected.

                    xResponses.Add (xResponse);

#pragma warning disable CS8602 // Suppresses warnings for dereferencing a possibly null reference.
#pragma warning disable CS8629 // Suppresses warnings for accessing the Value of a possibly null nullable type.
                    // The following code assumes that 'Choices', the first element in 'Choices', 'Index', and 'Delta' are all non-null,
                    // as they are validated before this section of code.
                    // The 'Content' property, however, can be null, and this is acceptable in this context.
                    int xIndex = xResponse.Choices [0].Index.Value;
                    string? xContent = xResponse.Choices [0].Delta.Content as string; // We know Content cant be an IEnumerable.
#pragma warning restore CS8602
#pragma warning restore CS8629

                    xBuilders [xIndex].Append (xContent);

                    await onChunkRetrievedAsync (xIndex, xContent, cancellationToken).ConfigureAwait (false);
                }

                return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonStrings.ToArray (), xResponses.ToArray (), xBuilders.Select (x => x.ToString ()).ToArray ());
            }

            else
            {
                string? xJsonString = await xClient.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
                yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);
                return (IsSuccess: false, xSendingResult.RequestJsonString, [ xJsonString ], [ xResponse ], []);
            }
        }

        // Suppresses the warning about passing a literal string as a parameter to a method expecting a URI (CA2234).
        // This suppression is used when the string is intentionally passed as a URI,
        // and the context ensures it is valid and does not require explicit URI construction.
        [SuppressMessage ("Usage", "CA2234")]
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString, yyGptImagesResponse Response, string [] RevisedPrompts, byte [][] ImageBytes)> GenerateImagesAsync (
            yyGptImagesConnectionInfo connectionInfo, yyGptImagesRequest request, CancellationToken cancellationToken = default)
        {
            using yyGptImagesClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request, cancellationToken).ConfigureAwait (false);

            string xJsonString = await xClient.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
            yyGptImagesResponse xResponse = yyGptImagesResponseParser.Parse (xJsonString);

            if (xSendingResult.ResponseMessage.IsSuccessStatusCode)
            {
#pragma warning disable CS8604 // Suppresses warnings for passing a possibly null reference as an argument.
                // It is guaranteed that 'Data' is not null, and either 'x.Url' or 'x.B64Json' will be non-null (one will always have a value).
                // These properties are validated beforehand.
                var xTasks = xResponse.Data.Select (async x =>
                {
                    // If ResponseFormat is not specified, the API takes it as "url".
                    if (request.ResponseFormat == null || request.ResponseFormat.Equals ("url", StringComparison.OrdinalIgnoreCase))
                    {
                        using HttpClient xHttpClient = new ()
                        {
                            Timeout = TimeSpan.FromSeconds (connectionInfo.Timeout ?? yyGptImagesConnectionInfo.DefaultTimeout) // From derived class.
                        };

                        // The IsSuccessStatusCode property is not checked because it is more beneficial to retrieve and return whatever data is available.
                        // Note that GenerateImagesAsync does not adhere to the single-responsibility principle.
                        // Image retrieval connections may experience timeouts, and there is no guarantee that the returned Base64 strings are valid.
                        // For precise control over the image retrieval process, consider implementing a separate, dedicated logic for this functionality.

                        var xImageResponse = await xHttpClient.GetAsync (x.Url, cancellationToken).ConfigureAwait (false);
                        return await xImageResponse.Content.ReadAsByteArrayAsync (cancellationToken).ConfigureAwait (false);
                    }

                    else if (request.ResponseFormat.Equals ("b64_json", StringComparison.OrdinalIgnoreCase))
                        return Convert.FromBase64String (x.B64Json);

                    else throw new yyArgumentException ("The response format is invalid.");
                });
#pragma warning restore CS8604

#pragma warning disable CS8619 // Suppresses warnings for conversion of nullability in type annotations.
                // The API documentation indicates that 'RevisedPrompt' will not be null.
                string [] xRevisedPrompts = xResponse.Data.Select (x => x.RevisedPrompt).ToArray ();
#pragma warning restore CS8619

                byte [][] xImageBytes = await Task.WhenAll (xTasks).ConfigureAwait (false);

                return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonString, xResponse, xRevisedPrompts, xImageBytes);
            }

            else return (IsSuccess: false, xSendingResult.RequestJsonString, xJsonString, xResponse, [], []);
        }
    }
}
