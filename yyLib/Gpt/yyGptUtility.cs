using System.Text;

namespace yyLib
{
    public static class yyGptUtility
    {
        public static async Task <(string JsonString, string []? Messages, string? ErrorMessage)> GenerateMessagesAsync (yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request)
        {
            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request);

            // We dont check IsSuccessStatusCode yet.
            // Let's try retrieving the JSON string and parsing it first.
            // If an error occurs, an exception will be thrown.

            string? xJsonString = await xClient.ReadToEndAsync ();
            yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);

            // The values are retrieved not as they may be but as they are supposed to be.
            // If Choices (IList) is null for example, an exception must occur.

            if (xSendingResult.HttpResponseMessage.IsSuccessStatusCode)
                return (JsonString: xJsonString!, Messages: xResponse.Choices!.Select (x => x.Message!.Content!).ToArray (), ErrorMessage: null);

            else return (JsonString: xJsonString!, Messages: null, ErrorMessage: xResponse.Error!.Message);
        }

        // In the streaming mode, we receive a lot of JSON strings.
        // Only the one that should contain an error message in case of an error can be returned.

        public static async Task <(string? JsonString, string []? Messages, string? ErrorMessage)> GenerateMessagesChunksAsync (yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request, Func <int, string?, Task> onChunkRetrieved)
        {
            bool? xStream = request.Stream;
            request.Stream = true;

            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request);

            request.Stream = xStream;

            if (xSendingResult.HttpResponseMessage.IsSuccessStatusCode)
            {
                yyAutoExpandingList <StringBuilder> xBuilders = [];

                while (true)
                {
                    // Each line should look like "data: { ..." or "data: [DONE]".
                    string? xLine = await xClient.ReadLineAsync ();

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

                    int xIndex = xResponse.Choices! [0].Index!.Value;
                    string? xContent = xResponse.Choices! [0]!.Delta!.Content;
                    xBuilders [xIndex].Append (xContent);

                    await onChunkRetrieved (xIndex, xContent);
                }

                return (JsonString: null, Messages: xBuilders.Select (x => x.ToString ()).ToArray (), ErrorMessage: null);
            }

            else
            {
                string? xJsonString = await xClient.ReadToEndAsync ();
                yyGptChatResponse xResponse = yyGptChatResponseParser.Parse (xJsonString);
                return (JsonString: xJsonString, Messages: null, ErrorMessage: xResponse.Error!.Message);
            }
        }

        public static async Task <(string JsonString, byte [][]? Bytes, string? ErrorMessage)> GenerateImagesAsync (yyGptImagesConnectionInfo connectionInfo, yyGptImagesRequest request)
        {
            using yyGptImagesClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request);

            // Not checking IsSuccessStatusCode yet for the same reason as GenerateMessagesAsync.

            string? xJsonString = await xClient.ReadToEndAsync ();
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
                        var xResponseAlt = await xHttpClient.GetAsync (x.Url);

                        // Just to make sure.
                        xResponseAlt.EnsureSuccessStatusCode ();

                        return await xResponseAlt.Content.ReadAsByteArrayAsync ();
                    }

                    else throw new yyArgumentException ("The response format is invalid.");
                });

                byte [][] xBytes = await Task.WhenAll (xTasks);

                return (JsonString: xJsonString!, Bytes: xBytes, ErrorMessage: null);
            }

            else return (JsonString: xJsonString!, Bytes: null, ErrorMessage: xResponse.Error!.Message);
        }
    }
}
