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
    }
}
