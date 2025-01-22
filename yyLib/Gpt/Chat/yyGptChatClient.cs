using System.Text;
using System.Text.Json;

namespace yyLib
{
    public class yyGptChatClient: IDisposable
    {
        public yyGptChatConnectionInfo ConnectionInfo { get; private set; }

        public HttpClient? HttpClient { get; private set; }

        public HttpResponseMessage? ResponseMessage { get; private set; }

        public Stream? ResponseStream { get; private set; }

        public StreamReader? ResponseStreamReader { get; private set; }

        public yyGptChatClient (yyGptChatConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;

            HttpClient = new ()
            {
                Timeout = TimeSpan.FromSeconds (ConnectionInfo.Timeout ?? yyGptChatConnectionInfo.DefaultTimeout) // From derived class.
            };

            HttpClient.DefaultRequestHeaders.Authorization = new ("Bearer", ConnectionInfo.ApiKey);

            // https://platform.openai.com/docs/api-reference/organizations-and-projects-optional
            // https://github.com/openai/openai-dotnet/blob/main/src/Custom/OpenAIClient.cs

            if (string.IsNullOrWhiteSpace (ConnectionInfo.Organization) == false)
                HttpClient.DefaultRequestHeaders.Add ("OpenAI-Organization", ConnectionInfo.Organization);

            if (string.IsNullOrWhiteSpace (ConnectionInfo.Project) == false)
                HttpClient.DefaultRequestHeaders.Add ("OpenAI-Project", ConnectionInfo.Project);
        }

        public async Task <(HttpResponseMessage HttpResponseMessage, Stream Stream)> SendAsync (yyGptChatRequest request, CancellationToken cancellationToken = default)
        {
            if (HttpClient == null)
                throw new yyObjectDisposedException ($"'{nameof (HttpClient)}' is disposed.");

            var xJsonString = JsonSerializer.Serialize (request, yyJson.DefaultSerializationOptions);

            using StringContent xContent = new (xJsonString, Encoding.UTF8, "application/json");
            using HttpRequestMessage xMessage = new (HttpMethod.Post, ConnectionInfo.Endpoint) { Content = xContent };

            var xResponse = await HttpClient.SendAsync (xMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait (false);

            // Commented out to receive error messages.
            // xResponse.EnsureSuccessStatusCode ();

            // DisposeAsync is not available.
            ResponseMessage?.Dispose ();
            ResponseMessage = xResponse;

            if (ResponseStream != null)
                await ResponseStream.DisposeAsync ().ConfigureAwait (false);

            ResponseStream = await xResponse.Content.ReadAsStreamAsync (cancellationToken).ConfigureAwait (false);

            ResponseStreamReader?.Dispose ();
            ResponseStreamReader = new (ResponseStream);

            return (xResponse, ResponseStream);
        }

        public async Task <string?> ReadToEndAsync (CancellationToken cancellationToken = default)
        {
            if (ResponseStreamReader == null)
                throw new yyObjectDisposedException ($"'{nameof (ResponseStreamReader)}' is disposed.");

            if (ResponseStreamReader.EndOfStream)
                return await Task.FromResult <string?> (null).ConfigureAwait (false);

            return await ResponseStreamReader.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
        }

        public async ValueTask <string?> ReadLineAsync (CancellationToken cancellationToken = default)
        {
            if (ResponseStreamReader == null)
                throw new yyObjectDisposedException ($"'{nameof (ResponseStreamReader)}' is disposed.");

            if (ResponseStreamReader.EndOfStream)
                return await ValueTask.FromResult <string?> (null).ConfigureAwait (false);

            return await ResponseStreamReader.ReadLineAsync (cancellationToken).ConfigureAwait (false);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing)
            {
                HttpClient?.Dispose ();
                HttpClient = null;

                ResponseMessage?.Dispose ();
                ResponseMessage = null;

                ResponseStream?.Dispose ();
                ResponseStream = null;

                ResponseStreamReader?.Dispose ();
                ResponseStreamReader = null;
            }
        }

        public void Dispose ()
        {
            Dispose (true);

            // Prevents the garbage collector from calling the finalizer for this object.
            // This is used because the Dispose method has already cleaned up resources, making finalization unnecessary.
            // Suppressing finalization improves performance by avoiding redundant cleanup during garbage collection.
            // https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1816
            GC.SuppressFinalize (this);
        }
    }
}
