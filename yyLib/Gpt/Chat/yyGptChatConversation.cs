using yyLib;

namespace yyGptLib
{
    public class yyGptChatConversation (yyGptChatConnectionInfo connectionInfo): IDisposable
    {
        public yyGptChatClient Client { get; private set; } = new yyGptChatClient (connectionInfo);

        public yyGptChatRequest Request { get; private set; } = new yyGptChatRequest ();

        public async Task SendAsync (CancellationToken? cancellationTokenForSendAsync = null, CancellationToken? cancellationTokenForReadAsStreamAsync = null) =>
            await Client.SendAsync (Request, cancellationTokenForSendAsync, cancellationTokenForReadAsStreamAsync);

        // As a result of sugar coating, with or without a model class for the returned value, we anyway have to implement some conditioning.

        // If IsSuccess is true, Messages should contain one or more messages.
        // If IsSuccess is false, if an error message was retrieved, Messages should have only one element, which is the error message.
        // If IsSuccess is false and Messages is empty, an Exception instance should be available.

        // Added RawContent to see what exactly is returned from the server.

        /// <summary>
        /// Add one of the returned messages to the request to continue the conversation.
        /// </summary>
        public async Task <(bool IsSuccess, string? RawContent, IList <string> Messages, Exception? Exception)> TryReadAndParseAsync (CancellationToken? cancellationToken = null)
        {
            string? xJson = null;

            try
            {
                xJson = await Client.ReadToEndAsync (cancellationToken);
                var xResponse = yyGptChatResponseParser.Parse (xJson);

                if (Client.ResponseMessage!.IsSuccessStatusCode)
                    return (true, xJson, xResponse.Choices!.Select (x => x.Message!.Content!).ToList (), null);

                else return (false, xJson, new [] { xResponse.Error!.Message! }, null);
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
                return (false, xJson, new List <string> (), xException);
            }
        }

        // If IsSuccess is true,
            // if PartialMessage isnt null, use it and continue reading.
            // if PartialMessage is null, discard it and stop reading.

        // If IsSuccess is false,
            // if PartialMessage isnt null, it should be an error message returned from the server.
            // if PartialMessage is null, an Exception instance should be available.

        // Added RawContent.

        /// <summary>
        /// Add one of the returned messages to the request to continue the conversation.
        /// </summary>
        public async Task <(bool IsSuccess, string? RawContent, int Index, string? PartialMessage, Exception? Exception)> TryReadAndParseChunkAsync (CancellationToken? cancellationToken = null)
        {
            string? xLine = null,
                xJson = null;

            try
            {
                // Feels a little redundant, but the cost is negligible.
                // I dont want to move this check to SendAsync sacrificing the consistency.
                // An error, if it arises, should be returned from the reading code.

                if (Client.ResponseMessage!.IsSuccessStatusCode)
                {
                    xLine = await Client.ReadLineAsync (cancellationToken);

                    if (xLine == null)
                        return (true, xLine, default, null, null); // End of stream.
                        // We usually dont get here as "data: [DONE]" is detected before.

                    // If a returned line is empty and doesnt contain the "data: " part,
                    // we consider an empty partial message has been retrieved and continue for "data: [DONE]" or the end of stream.

                    if (string.IsNullOrWhiteSpace (xLine))
                        return (true, xLine, default, string.Empty, null);

                    var xResponse = yyGptChatResponseParser.ParseChunk (xLine);

                    if (xResponse == yyGptChatResponse.Empty)
                        return (true, xLine, default, null, null); // "data: [DONE]" is detected.

                    int xIndex = xResponse.Choices! [0].Index!.Value;
                    string? xContent = xResponse.Choices! [0].Delta!.Content;

                    return (true, xLine, xIndex, xContent, null);
                }

                else
                {
                    xJson = await Client.ReadToEndAsync (cancellationToken);
                    var xResponse = yyGptChatResponseParser.Parse (xJson);

                    return (false, xJson, default, xResponse.Error!.Message, null);
                }
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
                // Regardless of where the exception is thrown, this should work just fine.
                return (false, xLine ?? xJson, default, null, xException);
            }
        }

        public void Dispose ()
        {
            Client.Dispose (); // Could be called a number of times.
            GC.SuppressFinalize (this);
        }
    }
}
