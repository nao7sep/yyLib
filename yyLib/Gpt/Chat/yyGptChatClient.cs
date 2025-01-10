using System.Net.Http.Headers;
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

            HttpClient = new HttpClient ();

            if (ConnectionInfo.Timeout != null)
                HttpClient.Timeout = TimeSpan.FromSeconds (ConnectionInfo.Timeout.Value);

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", ConnectionInfo.ApiKey);

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

            using (var xContent = new StringContent (xJsonString, Encoding.UTF8, "application/json"))
            using (var xMessage = new HttpRequestMessage (HttpMethod.Post, ConnectionInfo.Endpoint) { Content = xContent })
            {
                var xResponse = await HttpClient.SendAsync (xMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                // Commented out to receive error messages.
                // xResponse.EnsureSuccessStatusCode ();

                ResponseMessage?.Dispose ();
                ResponseMessage = xResponse;

                ResponseStream?.Dispose ();
                ResponseStream = await xResponse.Content.ReadAsStreamAsync (cancellationToken);

                ResponseStreamReader?.Dispose ();
                ResponseStreamReader = new StreamReader (ResponseStream);

                return (xResponse, ResponseStream);
            }
        }

        public async Task <string?> ReadToEndAsync (CancellationToken cancellationToken = default)
        {
            if (ResponseStreamReader == null)
                throw new yyObjectDisposedException ($"'{nameof (ResponseStreamReader)}' is disposed.");

            if (ResponseStreamReader.EndOfStream)
                return await Task.FromResult <string?> (null);

            return await ResponseStreamReader.ReadToEndAsync (cancellationToken);
        }

        public async ValueTask <string?> ReadLineAsync (CancellationToken cancellationToken = default)
        {
            if (ResponseStreamReader == null)
                throw new yyObjectDisposedException ($"'{nameof (ResponseStreamReader)}' is disposed.");

            if (ResponseStreamReader.EndOfStream)
                return await ValueTask.FromResult <string?> (null);

            return await ResponseStreamReader.ReadLineAsync (cancellationToken);
        }

        public void Dispose ()
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
}
