﻿using System.Text;
using System.Text.Json;

namespace yyLib
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

            HttpClient = new ()
            {
                Timeout = TimeSpan.FromSeconds (ConnectionInfo.Timeout ?? yyGptImagesConnectionInfo.DefaultTimeout) // From derived class.
            };

            HttpClient.DefaultRequestHeaders.Authorization = new ("Bearer", ConnectionInfo.ApiKey);

            if (string.IsNullOrWhiteSpace (ConnectionInfo.Organization) == false)
                HttpClient.DefaultRequestHeaders.Add ("OpenAI-Organization", ConnectionInfo.Organization);

            if (string.IsNullOrWhiteSpace (ConnectionInfo.Project) == false)
                HttpClient.DefaultRequestHeaders.Add ("OpenAI-Project", ConnectionInfo.Project);
        }

        public async Task <(string RequestJsonString, HttpResponseMessage ResponseMessage, Stream ResponseStream)> SendAsync (
            yyGptImagesRequest request, CancellationToken cancellationToken = default)
        {
            if (HttpClient == null)
                throw new yyObjectDisposedException ($"'{nameof (HttpClient)}' is disposed.");

            var xJsonString = JsonSerializer.Serialize (request, yyJson.DefaultSerializationOptions);

            using StringContent xContent = new (xJsonString, Encoding.UTF8, "application/json"); // Must be UTF-8.
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

            return (xJsonString, xResponse, ResponseStream);
        }

        public async Task <string> ReadToEndAsync (CancellationToken cancellationToken = default)
        {
            if (ResponseStreamReader == null)
                throw new yyObjectDisposedException ($"'{nameof (ResponseStreamReader)}' is disposed.");

            // GitHub Copilot suggested the following code,
            // but I'm still going to use the well-tested piece from yyGptChatClient.
            // Worst case scenario, it'll be just harmlessly redundant.

            // Added: If we call ReadToEnd at the end of a stream, we get an empty string.
            // The following code used to return null.
            // We dont need to receive null to know that the stream has ended.

            if (ResponseStreamReader.EndOfStream)
                return await Task.FromResult (string.Empty).ConfigureAwait (false);

            return await ResponseStreamReader.ReadToEndAsync (cancellationToken).ConfigureAwait (false);
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
