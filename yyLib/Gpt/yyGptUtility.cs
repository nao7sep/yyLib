using System.Text;

namespace yyLib
{
    public static class yyGptUtility
    {
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString, yyGptChatResponse Response, string? []? Messages)> GenerateMessagesAsync (
            yyGptChatClient client, yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            var xSendingResult = await client.SendAsync (request, cancellationToken).ConfigureAwait (false);
            string xJsonString = await client.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
            yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);

            return (xSendingResult.ResponseMessage.IsSuccessStatusCode, xSendingResult.RequestJsonString, xJsonString, xResponse,
                xResponse.Choices?.Select (x => x.Message?.Content as string).ToArray ()); // Content is guaranteed to be a string or null.
        }

        /// <summary>
        /// Consider using the overload that accepts a yyGptChatClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString, yyGptChatResponse Response, string? []? Messages)> GenerateMessagesAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            using var xClient = new yyGptChatClient (connectionInfo);
            return await GenerateMessagesAsync (xClient, request, cancellationToken).ConfigureAwait (false);
        }

        // https://github.com/nao7sep/Resources/blob/main/Documents/AI-Generated%20Notes/Understanding%20Async%20Lambdas%20and%20Task.CompletedTask%20in%20C%23.md

        // Design choice:
        // Messages could be a nullable list of non-nullable strings, but then it would be inconsistent with GenerateMessagesAsync.
        // Within the while loop, if a specific index never appears, the corresponding message should be null.
        // It is highly unlikely that an empty message or null is returned for a valid index,
        // making it safe to convert built and still empty messages to null.

        /// <summary>
        /// Numbers of responses and messages may differ.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string [] ResponseJsonStrings, yyGptChatResponse [] Responses, string? []? Messages)> GenerateMessagesChunksAsync (
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
                        throw new yyInvalidDataException ("The stream ended unexpectedly.");

                    if (string.IsNullOrWhiteSpace (xLine))
                        continue; // Continues for "data: [DONE]".

                    var xParsed = yyGptChatResponseParser.ParseChunk (xLine);

                    if (xParsed.Response == null)
                        break; // "data: [DONE]" is detected.

                    xJsonStrings.Add (xParsed.JsonString ?? throw new yyUnexpectedNullException ($"'{nameof (xParsed.JsonString)}' is null."));
                    xResponses.Add (xParsed.Response);

                    var xFirstChoice = xParsed.Response.Choices?.FirstOrDefault ();
                    int? xIndex = xFirstChoice?.Index;

                    if (xIndex != null)
                    {
                        // Content is guaranteed to be a string or null.
                        string? xContent = xFirstChoice?.Delta?.Content as string;
                        xBuilders [xIndex.Value].Append (xContent);
                        await onChunkRetrievedAsync (xIndex.Value, xContent, cancellationToken).ConfigureAwait (false);
                    }
                }

                string? [] xMessages = xBuilders.Select (x => x.ToString ().EmptyToNull ()).ToArray (); // Empty messages are converted to null.
                return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonStrings.ToArray (), xResponses.ToArray (), xMessages);
            }

            else
            {
                string xJsonString = await client.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
                yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);
                return (IsSuccess: false, xSendingResult.RequestJsonString, [ xJsonString ], [ xResponse ], Messages: null);
            }
        }

        /// <summary>
        /// Consider using the overload that accepts a yyGptChatClient instance for better performance.
        /// Numbers of responses and messages may differ.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string [] ResponseJsonStrings, yyGptChatResponse [] Responses, string? []? Messages)> GenerateMessagesChunksAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, Func <int, string?, CancellationToken, Task> onChunkRetrievedAsync, CancellationToken cancellationToken = default)
        {
            using var xClient = new yyGptChatClient (connectionInfo);
            return await GenerateMessagesChunksAsync (xClient, request, onChunkRetrievedAsync, cancellationToken).ConfigureAwait (false);
        }

        /// <summary>
        /// Only one of Urls or ImagesBytes will contain data while the other will be null.
        /// This method does not catch exceptions internally, so the caller must handle potential errors.
        /// RevisedPrompts will be null when using DALL-E 2, which doesnt return revised prompts.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString,
            yyGptImagesResponse Response, string? []? RevisedPrompts, string? []? Urls, byte []?[]? ImagesBytes)> GenerateImagesAsync (
            yyGptImagesClient client, yyGptImagesRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ResponseFormat != null &&
                request.ResponseFormat.Equals ("url", StringComparison.OrdinalIgnoreCase) == false &&
                request.ResponseFormat.Equals ("b64_json", StringComparison.OrdinalIgnoreCase) == false)
                    throw new yyArgumentException ("The response format is invalid.");

            var xSendingResult = await client.SendAsync (request, cancellationToken).ConfigureAwait (false);
            string xJsonString = await client.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
            yyGptImagesResponse xResponse = yyGptImagesResponseParser.Parse (xJsonString);

            if (xSendingResult.ResponseMessage.IsSuccessStatusCode)
            {
                string? []? xRevisedPrompts = xResponse.Data?.Select (x => x.RevisedPrompt).ToArray ();

                if (request.ResponseFormat == null || request.ResponseFormat.Equals ("url", StringComparison.OrdinalIgnoreCase))
                {
                    string? []? xUrls = xResponse.Data?.Select (x => x.Url).ToArray ();
                    return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonString, xResponse, xRevisedPrompts, xUrls, ImagesBytes: null);
                }

                else // ResponseFormat has been checked beforehand.
                {
                    byte []?[]? xImagesBytes = xResponse.Data?.Select (x => x.B64Json != null ? Convert.FromBase64String (x.B64Json) : null).ToArray ();
                    return (IsSuccess: true, xSendingResult.RequestJsonString, xJsonString, xResponse, xRevisedPrompts, Urls: null, xImagesBytes);
                }
            }

            else return (IsSuccess: false, xSendingResult.RequestJsonString, xJsonString, xResponse, RevisedPrompts: null, Urls: null, ImagesBytes: null);
        }

        /// <summary>
        /// Consider using the overload that accepts a yyGptImagesClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, string RequestJsonString, string ResponseJsonString,
            yyGptImagesResponse Response, string? []? RevisedPrompts, string? []? Urls, byte []?[]? ImagesBytes)> GenerateImagesAsync (
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
        /// The HttpClient's Timeout must be set to be the same as the yyGptImagesConnectionInfo instance that initiated the image generation process or to yyGptImagesConnectionInfo.DefaultTimeout.
        /// This method does not handle exceptions internally.
        /// Callers should catch HttpRequestException for network errors and TaskCanceledException if the request is canceled.
        /// </summary>
        public static async Task <(bool IsSuccess, byte []? ImageBytes)> RetrieveImageBytesAsync (
            HttpClient client, string url, CancellationToken cancellationToken = default)
        {
            using var xImageResponse = await client.GetAsync (url, cancellationToken).ConfigureAwait (false);

            if (xImageResponse.IsSuccessStatusCode)
            {
                var xImageBytes = await xImageResponse.Content.ReadAsByteArrayAsync (cancellationToken).ConfigureAwait (false);
                return (IsSuccess: true, xImageBytes);
            }

            else return (IsSuccess: false, ImageBytes: null);
        }

        /// <summary>
        /// Consider using the overload that accepts an HttpClient instance for better performance.
        /// </summary>
        public static async Task <(bool IsSuccess, byte []? ImageBytes)> RetrieveImageBytesAsync (
            yyGptImagesConnectionInfo connectionInfo, string url, CancellationToken cancellationToken = default)
        {
            using var xClient = CreateImageRetrievalHttpClient (connectionInfo);
            return await RetrieveImageBytesAsync (xClient, url, cancellationToken).ConfigureAwait (false);
        }

        public static string BytesToUrl (string mimeType, byte [] bytes) =>
            $"data:{mimeType};base64,{Convert.ToBase64String (bytes)}";
    }
}
