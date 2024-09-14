using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using yyLib;

namespace yyGptLib
{
    public class yyGptImagesClient: IDisposable
    {
        public yyGptImagesConnectionInfo ConnectionInfo { get; private set; }

        public HttpClient? HttpClient { get; private set; }

        public HttpResponseMessage? ResponseMessage { get; private set; }

        public Stream? ResponseStream { get; private set; }

        public StreamReader? ResponseStreamReader { get; private set; }

        public yyGptImagesClient (yyGptImagesConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;

            HttpClient = new HttpClient ();

            if (ConnectionInfo.Timeout != null)
                HttpClient.Timeout = TimeSpan.FromSeconds (ConnectionInfo.Timeout.Value);

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", ConnectionInfo.ApiKey);

            if (string.IsNullOrWhiteSpace (ConnectionInfo.Organization) == false)
                HttpClient.DefaultRequestHeaders.Add ("OpenAI-Organization", ConnectionInfo.Organization);

            if (string.IsNullOrWhiteSpace (ConnectionInfo.Project) == false)
                HttpClient.DefaultRequestHeaders.Add ("OpenAI-Project", ConnectionInfo.Project);
        }

        public async Task <(HttpResponseMessage HttpResponseMessage, Stream Stream)> SendAsync (yyGptImagesRequest request,
            CancellationToken? cancellationTokenForSendAsync = null, CancellationToken? cancellationTokenForReadAsStreamAsync = null)
        {
            if (HttpClient == null)
                throw new yyObjectDisposedException ($"'{nameof (HttpClient)}' is disposed.");

            var xJson = JsonSerializer.Serialize (request, yyJson.DefaultSerializationOptions);

            using (var xContent = new StringContent (xJson, Encoding.UTF8, "application/json"))
            using (var xMessage = new HttpRequestMessage (HttpMethod.Post, ConnectionInfo.Endpoint) { Content = xContent })
            {
                var xResponse = await HttpClient.SendAsync (xMessage, HttpCompletionOption.ResponseHeadersRead, cancellationTokenForSendAsync ?? CancellationToken.None);

                // Commented out to receive error messages.
                // xResponse.EnsureSuccessStatusCode ();

                ResponseMessage?.Dispose ();
                ResponseMessage = xResponse;

                ResponseStream?.Dispose ();
                ResponseStream = await xResponse.Content.ReadAsStreamAsync (cancellationTokenForReadAsStreamAsync ?? CancellationToken.None);

                ResponseStreamReader?.Dispose ();
                ResponseStreamReader = new StreamReader (ResponseStream);

                return (xResponse, ResponseStream);
            }
        }

        public async Task <string?> ReadToEndAsync (CancellationToken? cancellationToken = null)
        {
            if (ResponseStreamReader == null)
                throw new yyObjectDisposedException ($"'{nameof (ResponseStreamReader)}' is disposed.");

            // GitHub Copilot suggested the following code,
            //     but I'm still going to use the well-tested piece from yyGptChatClient.
            // Worst case scenario, it'll be just harmlessly redundant.
            // return await ResponseStreamReader.ReadToEndAsync ();

            if (ResponseStreamReader.EndOfStream)
                return await Task.FromResult <string?> (null);

            return await ResponseStreamReader.ReadToEndAsync (cancellationToken ?? CancellationToken.None);
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

            GC.SuppressFinalize (this);
        }
    }
}
