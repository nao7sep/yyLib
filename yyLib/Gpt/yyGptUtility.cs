using System.Text;

namespace yyLib
{
    public static class yyGptUtility
    {
        public static async Task <(string JsonString, string []? GeneratedMessages, string? ErrorMessage)> GenerateMessagesAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request, cancellationToken);

            // We dont check IsSuccessStatusCode yet.
            // Let's try retrieving the JSON string and parsing it first.
            // If an error occurs, an exception will be thrown.

            string? xJsonString = await xClient.ReadToEndAsync (cancellationToken);
            yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);

            // The values are retrieved not as they may be but as they are supposed to be.
            // If Choices (IList) is null for example, an exception must occur.

            if (xSendingResult.HttpResponseMessage.IsSuccessStatusCode)
                // If Parse didnt throw a yyFormatException, the extracted values are supposed to be valid.
                return (JsonString: xJsonString!, GeneratedMessages: xResponse.Choices!.Select (x => x.Message!.Content!).ToArray (), ErrorMessage: null);
            else return (JsonString: xJsonString!, GeneratedMessages: null, ErrorMessage: xResponse.Error!.Message);
        }

        // In the streaming mode, we receive a lot of JSON strings.
        // Only the one that should contain an error message in case of an error can be returned.

        /// <summary>
        /// onChunkRetrieved should be a light-weight method that would return quickly.
        /// </summary>
        public static async Task <(string? JsonString, string []? GeneratedMessages, string? ErrorMessage)> GenerateMessagesChunksAsync (
            yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, Func <int, string?, Task> onChunkRetrieved, CancellationToken cancellationToken = default)
        {
            bool? xStream = request.Stream;
            request.Stream = true;

            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request, cancellationToken);

            request.Stream = xStream;

            if (xSendingResult.HttpResponseMessage.IsSuccessStatusCode)
            {
                yyAutoExpandingList <StringBuilder> xBuilders = [];

                while (true)
                {
                    // Each line should look like "data: { ..." or "data: [DONE]".
                    string? xLine = await xClient.ReadLineAsync (cancellationToken);

                    // If null is returned before "data: [DONE]", something is wrong.
                    // It is highly unlikely that all the data is received and only the last line is missing.

                    if (xLine == null)
                        throw new yyFormatException ("The stream ended unexpectedly.");

                    if (string.IsNullOrWhiteSpace  (xLine))
                        continue; // Continues for "data: [DONE]".

                    var xResponse = yyGptChatResponseParser.ParseChunk (xLine);

                    if (xResponse == yyGptChatResponse.Empty)
                        break; // "data: [DONE]" is detected.

                    // Like in GenerateMessagesAsync, the values are retrieved as they are supposed to be.
                    // If ParseChunk succeeded, the extracted values should be valid.

                    int xIndex = xResponse.Choices! [0].Index!.Value;
                    string? xContent = xResponse.Choices! [0]!.Delta!.Content;
                    xBuilders [xIndex].Append (xContent);

                    await onChunkRetrieved (xIndex, xContent);
                }

                return (JsonString: null, GeneratedMessages: xBuilders.Select (x => x.ToString ()).ToArray (), ErrorMessage: null);
            }

            else
            {
                string? xJsonString = await xClient.ReadToEndAsync (cancellationToken);
                yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);
                return (JsonString: xJsonString, GeneratedMessages: null, ErrorMessage: xResponse.Error!.Message);
            }
        }

        public static async Task <(string JsonString, byte [][]? ImageBytes, string? ErrorMessage)> GenerateImagesAsync (
            yyGptImagesConnectionInfo connectionInfo, yyGptImagesRequest request, CancellationToken cancellationToken = default)
        {
            using yyGptImagesClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request, cancellationToken);

            // Not checking IsSuccessStatusCode yet for the same reason as GenerateMessagesAsync.

            string? xJsonString = await xClient.ReadToEndAsync (cancellationToken);
            yyGptImagesResponse xResponse = yyGptImagesResponseParser.Parse (xJsonString);

            if (xSendingResult.HttpResponseMessage.IsSuccessStatusCode)
            {
                var xTasks = xResponse.Data!.Select (async x =>
                {
                    if (request.ResponseFormat!.Equals ("b64_json", StringComparison.OrdinalIgnoreCase))
                        return Convert.FromBase64String (x.B64Json!);

                    else if (request.ResponseFormat!.Equals ("url", StringComparison.OrdinalIgnoreCase))
                    {
                        using HttpClient xHttpClient = new ();
                        var xImageResponse = await xHttpClient.GetAsync (x.Url, cancellationToken);

                        // Just to make sure.
                        xImageResponse.EnsureSuccessStatusCode ();

                        return await xImageResponse.Content.ReadAsByteArrayAsync (cancellationToken);
                    }

                    else throw new yyArgumentException ("The response format is invalid.");
                });

                byte [][] xImageBytes = await Task.WhenAll (xTasks);

                return (JsonString: xJsonString!, ImageBytes: xImageBytes, ErrorMessage: null);
            }

            else return (JsonString: xJsonString!, ImageBytes: null, ErrorMessage: xResponse.Error!.Message);
        }
    }
}
