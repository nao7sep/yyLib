using System.Text;

namespace yyLib
{
    public static class yyGptUtility
    {
        public static async Task <(string JsonString, string []? Messages, string? ErrorMessage)> GenerateMessagesAsync (yyGptChatConnectionInfo connectionInfo, yyGptChatRequest request)
        {
            using yyGptChatClient xClient = new (connectionInfo);
            var xSendingResult = await xClient.SendAsync (request);
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
                    string? xLine = await xClient.ReadLineAsync ();

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
            string? xJsonString = await xClient.ReadToEndAsync ();
            yyGptImagesResponse xResponse = yyGptImagesResponseParser.Parse (xJsonString);

            if (xSendingResult.HttpResponseMessage.IsSuccessStatusCode)
            {
                // The files will be downloaded in a synchronous manner.
                // As GenerateImagesAsync should have been called in an asynchronous manner,
                // the synchronous download should not be a problem. => Tested.

                return (JsonString: xJsonString!, Bytes: xResponse.Data!.Select (x =>
                {
                    if (request.ResponseFormat!.Equals ("b64_json", StringComparison.OrdinalIgnoreCase))
                        return Convert.FromBase64String (x.B64Json!);

                    else if (request.ResponseFormat!.Equals ("url", StringComparison.OrdinalIgnoreCase))
                    {
                        using HttpClient xHttpClient = new ();
                        var xResponseAlt = xHttpClient.GetAsync (x.Url).Result;

                        // Just to make sure.
                        xResponseAlt.EnsureSuccessStatusCode ();

                        return xResponseAlt.Content.ReadAsByteArrayAsync ().Result;
                    }

                    else throw new yyArgumentException ("The response format is invalid.");
                }).
                ToArray (),
                ErrorMessage: null);
            }

            else return (JsonString: xJsonString!, Bytes: null, ErrorMessage: xResponse.Error!.Message);
        }
    }
}
