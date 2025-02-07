using System.Text;

namespace yyLib
{
    public static class yyGptUtility
    {
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString, yyGptChatResponse Response, string [] Messages)> GenerateMessagesAsync (
            yyGptChatClient client, yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            var xSendingResult = await client.SendAsync (request, cancellationToken).ConfigureAwait (false);

            string xJsonString = await client.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
            yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);
            yyGptChatValidator.ValidateMessagesResponse (xResponse, xJsonString);

#pragma warning disable CS8602 // Disables warnings for dereferencing a possibly null reference.
#pragma warning disable CS8604 // Disables warnings for passing a possibly null reference as an argument.
#pragma warning disable CS8619 // Disables warnings for conversion of nullability in type annotations.
            // The following code assumes that 'Choices', 'Message', and 'Content' are all non-null, as they are validated before this section of code.
            string [] xMessages = xResponse.Choices.Select (x => x.Message.Content as string).ToArray (); // Content is a string.
#pragma warning restore CS8602
#pragma warning restore CS8604
#pragma warning restore CS8619

            return (xSendingResult.ResponseMessage.IsSuccessStatusCode, xSendingResult.RequestJsonString, xJsonString, xResponse, xMessages);
        }

        /// <summary>
        /// Consider using the overload that accepts a yyGptChatClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString, yyGptChatResponse Response, string [] Messages)> GenerateMessagesAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            using var xClient = new yyGptChatClient (connectionInfo);
            return await GenerateMessagesAsync (xClient, request, cancellationToken).ConfigureAwait (false);
        }

        // Refer to "async-lambdas-task-completedtask-csharp.md" for more information relevant to onChunkRetrievedAsync.

        public static async Task <(bool IsSuccess, string RequestJsonString, string [] ResponseJsonStrings, yyGptChatResponse [] Responses, string [] Messages)> GenerateMessagesChunksAsync (
            yyGptChatClient client, yyGptChatRequest request, Func <int, string?, CancellationToken, Task> onChunkRetrievedAsync, CancellationToken cancellationToken = default)
        {
            if (request.Stream is null or false)
                throw new yyArgumentException ("The request must be streaming.");

            var xSendingResult = await client.SendAsync (request, cancellationToken).ConfigureAwait (false);

            if (xSendingResult.ResponseMessage.IsSuccessStatusCode)
            {
                List <string> xJsonStrings = [];
                List <yyGptChatResponse> xResponses = [];
                yyAutoExpandingList <StringBuilder> xBuilders = [];

                while (true)
                {
                    // Each line should look like "data: { ..." or "data: [DONE]".
                    string? xLine = await client.ReadLineAsync (cancellationToken).ConfigureAwait (false);

                    // If null is returned before "data: [DONE]", something is wrong.
                    // It is highly unlikely that all the data is received and only the last line is missing.

                    if (xLine == null)
                        throw new yyFormatException ("The stream ended unexpectedly.");

                    if (string.IsNullOrWhiteSpace  (xLine))
                        continue; // Continues for "data: [DONE]".

                    xJsonStrings.Add (xLine);

                    var xResponse = yyGptChatResponseParser.ParseChunk (xLine);
                    yyGptChatValidator.ValidateMessageChunkResponse (xResponse, xLine);

                    if (xResponse == yyGptChatResponse.Empty)
                        break; // "data: [DONE]" is detected.

                    xResponses.Add (xResponse);

#pragma warning disable CS8602 // Suppresses warnings for dereferencing a possibly null reference.
#pragma warning disable CS8629 // Suppresses warnings for accessing the Value of a possibly null nullable type.
                    // The following code assumes that 'Choices', the first element in 'Choices', 'Index', and 'Delta' are all non-null,
                    // as they are validated before this section of code.
                    // The 'Content' property, however, can be null, and this is acceptable in this context.
                    int xIndex = xResponse.Choices [0].Index.Value;
                    string? xContent = xResponse.Choices [0].Delta.Content as string; // Content is a string.
#pragma warning restore CS8602
#pragma warning restore CS8629

                    xBuilders [xIndex].Append (xContent);

                    await onChunkRetrievedAsync (xIndex, xContent, cancellationToken).ConfigureAwait (false);
                }

                return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonStrings.ToArray (), xResponses.ToArray (), xBuilders.Select (x => x.ToString ()).ToArray ());
            }

            else
            {
                string? xJsonString = await client.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
                yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);
                yyGptChatValidator.ValidateMessagesResponse (xResponse, xJsonString);
                return (IsSuccess: false, xSendingResult.RequestJsonString, [ xJsonString ], [ xResponse ], []);
            }
        }

        /// <summary>
        /// Consider using the overload that accepts a yyGptChatClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string [] ResponseJsonStrings, yyGptChatResponse [] Responses, string [] Messages)> GenerateMessagesChunksAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, Func <int, string?, CancellationToken, Task> onChunkRetrievedAsync, CancellationToken cancellationToken = default)
        {
            using var xClient = new yyGptChatClient (connectionInfo);
            return await GenerateMessagesChunksAsync (xClient, request, onChunkRetrievedAsync, cancellationToken).ConfigureAwait (false);
        }

        /// <summary>
        /// Only one of Urls or ImageBytes will contain data, while the other will be an empty array.
        /// This method does not catch exceptions internally, so the caller must handle potential errors.
        /// RevisedPrompts will be an empty array when using the DALL-E 2 model, which doesnt return revised prompts.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString,
            yyGptImagesResponse Response, string [] RevisedPrompts, string [] Urls, byte [][] ImageBytes)> GenerateImagesAsync (
            yyGptImagesClient client, yyGptImagesRequest request, CancellationToken cancellationToken = default)
        {
            var xSendingResult = await client.SendAsync (request, cancellationToken).ConfigureAwait (false);

            string xJsonString = await client.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
            yyGptImagesResponse xResponse = yyGptImagesResponseParser.Parse (xJsonString);
            yyGptImagesValidator.ValidateImagesResponse (xResponse, xJsonString);

            if (xSendingResult.ResponseMessage.IsSuccessStatusCode)
            {
#pragma warning disable CS8604 // Suppresses warnings for passing a possibly null reference as an argument.
                // It is guaranteed that 'Data' is not null, and either 'x.Url' or 'x.B64Json' will be non-null (one will always have a value).
                // These properties are validated beforehand.
#pragma warning disable CS8619 // Suppresses warnings for conversion of nullability in type annotations.
                // The API documentation indicates that 'RevisedPrompt' will not be null when DALL-E 3 model is used.
                // When DALL-E 2 model is used, 'RevisedPrompt' will be null, leaving us no choice but to return an empty array.

                string [] xRevisedPrompts = xResponse.Data.All (x => x.RevisedPrompt != null) ? xResponse.Data.Select (x => x.RevisedPrompt).ToArray () : [];

                if (request.ResponseFormat == null || request.ResponseFormat.Equals ("url", StringComparison.OrdinalIgnoreCase))
                {
                    string [] xUrls = xResponse.Data.Select (x => x.Url).ToArray ();
                    return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonString, xResponse, xRevisedPrompts, xUrls, []);
                }

                else if (request.ResponseFormat.Equals ("b64_json", StringComparison.OrdinalIgnoreCase))
                {
                    byte [][] xImageBytes = xResponse.Data.Select (x => Convert.FromBase64String (x.B64Json)).ToArray ();
                    return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonString, xResponse, xRevisedPrompts, [], xImageBytes);
                }

                else throw new yyArgumentException ("The response format is invalid.");

#pragma warning restore CS8604
#pragma warning restore CS8619
            }

            else return (IsSuccess: false, xSendingResult.RequestJsonString, xJsonString, xResponse, [], [], []);
        }

        /// <summary>
        /// Consider using the overload that accepts a yyGptImagesClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString,
            yyGptImagesResponse Response, string [] RevisedPrompts, string [] Urls, byte [][] ImageBytes)> GenerateImagesAsync (
            yyGptImagesConnectionInfo connectionInfo, yyGptImagesRequest request, CancellationToken cancellationToken = default)
        {
            using var xClient = new yyGptImagesClient (connectionInfo);
            return await GenerateImagesAsync (xClient, request, cancellationToken).ConfigureAwait (false);
        }

        /// <summary>
        /// Use "using" to ensure that the HttpClient is disposed of properly.
        /// </summary>
        public static HttpClient CreateImageRetrievalHttpClient (yyGptImagesConnectionInfo connectionInfo)
        {
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds (connectionInfo.Timeout ?? yyGptImagesConnectionInfo.DefaultTimeout)
            };
        }

        /// <summary>
        /// The HttpClient's Timeout must be set to match the yyGptImagesConnectionInfo instance
        /// that initiated the image generation process or to yyGptImagesConnectionInfo.DefaultTimeout.
        /// This method does not handle exceptions internally.
        /// Callers should catch HttpRequestException for network errors and TaskCanceledException if the request is canceled.
        /// </summary>
        public static async Task <(bool IsSuccess, byte [] ImageBytes)> RetrieveImageBytesAsync (
            HttpClient client, string url, CancellationToken cancellationToken = default)
        {
            using var xImageResponse = await client.GetAsync (url, cancellationToken).ConfigureAwait (false);

            if (xImageResponse.IsSuccessStatusCode)
            {
                var xImageBytes = await xImageResponse.Content.ReadAsByteArrayAsync (cancellationToken).ConfigureAwait (false);
                return (IsSuccess: true, xImageBytes);
            }

            else return (IsSuccess: false, []);
        }

        /// <summary>
        /// Consider using the overload that accepts an HttpClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, byte [] ImageBytes)> RetrieveImageBytesAsync (
            yyGptImagesConnectionInfo connectionInfo, string url, CancellationToken cancellationToken = default)
        {
            using var xClient = CreateImageRetrievalHttpClient (connectionInfo);
            return await RetrieveImageBytesAsync (xClient, url, cancellationToken).ConfigureAwait (false);
        }
    }
}
