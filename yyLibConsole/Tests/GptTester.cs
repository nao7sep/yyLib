using System.Diagnostics.CodeAnalysis;
using System.Text;
using yyLib;

namespace yyLibConsole
{
    internal static class GptTester
    {
        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void TestInteractionsOfGptModelsAndGeneratingImages (string firstAssistantDeveloperMessage, string secondAssistantDeveloperMessage, int interactionCount)
        {
            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            yyGptImagesConnectionInfo xImagesConnectionInfo = yyGptImagesConnectionInfo.Default;

            using var xFirstAssistantClient = new yyGptChatClient (xConnectionInfo);
            using var xSecondAssistantClient = new yyGptChatClient (xConnectionInfo);
            using var xImagesClient = new yyGptImagesClient (xImagesConnectionInfo);

            yyGptChatRequest xFirstAssistantRequest = new () { Model = yyGptChat.DefaultModel },
                             xSecondAssistantRequest = new () { Model = yyGptChat.DefaultModel };

            xFirstAssistantRequest.AddDeveloperMessage (firstAssistantDeveloperMessage);
            xSecondAssistantRequest.AddDeveloperMessage (secondAssistantDeveloperMessage);

            xSecondAssistantRequest.Stream = true;

            // Reusing a single HttpClient instance improves performance and avoids issues like socket exhaustion.
            // Creating a new HttpClient for each image retrieval would add unnecessary overhead and slow down the process.
            // By using a pre-created HttpClient, we keep connections efficient and make better use of system resources.
            using HttpClient xHttpClient = yyGptUtility.CreateImageRetrievalHttpClient (xImagesConnectionInfo);

            for (int temp = 0; temp < interactionCount; temp ++)
            {
                var xFirstAssistantResponse = yyGptUtility.GenerateMessagesAsync (xFirstAssistantClient, xFirstAssistantRequest).Result;

                if (xFirstAssistantResponse.IsSuccess == false)
                {
                    Console.WriteLine ($"First Assistant: {(xFirstAssistantResponse.Response.Error?.Message).GetVisibleString ()}");
                    break;
                }

                string xMessage = xFirstAssistantResponse.Messages [0];
                Console.WriteLine ($"First Assistant: {xMessage}");

                xFirstAssistantRequest.AddAssistantMessage (xMessage);
                xSecondAssistantRequest.AddUserMessage (xMessage);

                static Task _OnChunkRetrievedAsync (int index, string? content, CancellationToken cancellationToken)
                {
                    Console.Write (content);
                    return Task.CompletedTask;
                }

                Console.Write ("Second Assistant: ");

                // Refer to "async-lambdas-task-completedtask-csharp.md" for more information about the async lambda syntax.

                var xSecondAssistantResponse = yyGptUtility.GenerateMessagesChunksAsync (xSecondAssistantClient, xSecondAssistantRequest,
                    async (index, content, cancellationToken) => await _OnChunkRetrievedAsync (index, content, cancellationToken)).Result;

                if (xSecondAssistantResponse.IsSuccess == false)
                {
                    Console.WriteLine ((xSecondAssistantResponse.Responses [0].Error?.Message).GetVisibleString ());
                    break;
                }

                Console.WriteLine ();

                xMessage = xSecondAssistantResponse.Messages [0];

                xFirstAssistantRequest.AddUserMessage (xMessage);
                xSecondAssistantRequest.AddAssistantMessage (xMessage);

                if ((temp + 1) % 5 == 0) // 4, 9, 14...
                {
                    yyGptImagesRequest xImagesRequest = new ()
                    {
                        // https://platform.openai.com/docs/guides/images

                        Prompt = xMessage,
                        Model = yyGptImages.DefaultModel,
                        Quality = yyGptImages.DefaultQuality,
                        Size = yyGptImages.DefaultSize,
                        Style = yyGptImages.DefaultStyle
                    };

                    if ((temp + 6) % 10 == 0) // 4, 14...
                        xImagesRequest.ResponseFormat = "url";
                    else xImagesRequest.ResponseFormat = "b64_json";

                    Console.WriteLine ($"Generating image ({xImagesRequest.ResponseFormat})...");

                    var xImagesResponse = yyGptUtility.GenerateImagesAsync (xImagesClient, xImagesRequest).Result;

                    if (xImagesResponse.IsSuccess == false)
                    {
                        Console.WriteLine ($"Image generation failed: {(xImagesResponse.Response.Error?.Message).GetVisibleString ()}");
                        break;
                    }

                    byte [] xImageBytes = [];

                    if (xImagesRequest.ResponseFormat.Equals ("url", StringComparison.OrdinalIgnoreCase))
                    {
                        var xImageRetrievalResponse = yyGptUtility.RetrieveImageBytesAsync (xHttpClient, xImagesResponse.Urls [0]).Result;

                        if (xImageRetrievalResponse.IsSuccess == false)
                        {
                            Console.WriteLine ($"Image retrieval failed: {xImagesResponse.Urls [0].GetVisibleString ()}");
                            break;
                        }

                        xImageBytes = xImageRetrievalResponse.ImageBytes;
                    }

                    else xImageBytes = xImagesResponse.ImageBytes [0];

                    string xImagePartialFilePath = yyPath.Join (yySpecialDirectories.Desktop,
                               "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (DateTime.UtcNow)),
                           xImageFilePath = xImagePartialFilePath + ".png",
                           xPromptsFilePath = xImagePartialFilePath + ".txt";

                    File.WriteAllBytes (xImageFilePath, xImageBytes);

                    // This code uses AppendLine, which appends a string followed by a newline character sequence.
                    // The newline sequence is determined by the current environment (Environment.NewLine),
                    // making the output platform-dependent (e.g., "\n" on Linux/macOS, "\r\n" on Windows).

                    StringBuilder xPrompts = new ();
                    xPrompts.AppendLine ("[Original Prompt]");
                    xPrompts.AppendLine (xImagesRequest.Prompt);
                    xPrompts.AppendLine ();
                    xPrompts.AppendLine ("[Revised Prompt]");
                    xPrompts.AppendLine (xImagesResponse.RevisedPrompts [0]); // Unsafe when DALL-E 2 is used.

                    File.WriteAllText (xPromptsFilePath, xPrompts.ToString (), yyEncoding.DefaultEncoding);

                    Console.WriteLine ($"Image saved: {xImageFilePath}");
                    Console.WriteLine ($"Prompts saved: {xPromptsFilePath}");
                }
            }
        }

        public static void TestGeneratingMultipleMessagesAndImages (int messageCount, int imageCount)
        {
            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            using var xClient = new yyGptChatClient (xConnectionInfo);

            yyGptChatRequest xRequest = new ()
            {
                Model = yyGptChat.DefaultModel,
                N = messageCount
            };

            xRequest.AddUserMessage ("Please generate one short story in one paragraph.");

            var xFirstResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

            if (xFirstResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Message generation failed: {(xFirstResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            for (int temp = 0; temp < messageCount; temp ++)
                Console.WriteLine ($"Message {temp + 1}: {xFirstResponse.Messages [temp]}");

            xRequest.Stream = true;

            static Task _OnChunkRetrievedAsync (int index, string? content, CancellationToken cancellationToken)
            {
                Console.WriteLine ($"Message {index + 1}: {content}");
                return Task.CompletedTask;
            }

            // The following redundant lambda is for demonstration purposes.
            // Refer to "async-lambdas-task-completedtask-csharp.md" for more information about the async lambda syntax.

            var xSecondResponse = yyGptUtility.GenerateMessagesChunksAsync (xClient, xRequest,
                async (index, content, cancellationToken) => await _OnChunkRetrievedAsync (index, content, cancellationToken)).Result;

            if (xSecondResponse.IsSuccess == false)
            {
                Console.WriteLine ((xSecondResponse.Responses [0].Error?.Message).GetVisibleString ());
                return;
            }

            for (int temp = 0; temp < messageCount; temp ++)
                Console.WriteLine ($"Message {temp + 1}: {xSecondResponse.Messages [temp]}");

            yyGptImagesConnectionInfo xImagesConnectionInfo = yyGptImagesConnectionInfo.Default;
            using var xImagesClient = new yyGptImagesClient (xImagesConnectionInfo);
            using var xHttpClient = yyGptUtility.CreateImageRetrievalHttpClient (xImagesConnectionInfo);

            yyGptImagesRequest xImagesRequest = new ()
            {
                // Quality and Style are not supported by DALL-E 2.
                // https://platform.openai.com/docs/guides/images
                // https://platform.openai.com/docs/api-reference/images/create

                Prompt = "Please generate random images.",
                Model = "dall-e-2", // Note that this model doesnt return revised prompts.
                Size = "1024x1024",
                N = imageCount,
                ResponseFormat = "url" // Default value specified explicitly.
            };

            var xImagesResponse = yyGptUtility.GenerateImagesAsync (xImagesClient, xImagesRequest).Result;

            if (xImagesResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Image generation failed: {(xImagesResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            // Time when images generation is completed.
            DateTime xUtcNow = DateTime.UtcNow;
            string xPartialFileName = "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (xUtcNow);

            for (int temp = 0; temp < imageCount; temp ++)
            {
                var xImageRetrievalResponse = yyGptUtility.RetrieveImageBytesAsync (xHttpClient, xImagesResponse.Urls [temp]).Result;

                if (xImageRetrievalResponse.IsSuccess == false)
                {
                    Console.WriteLine ($"Image retrieval failed: {xImagesResponse.Urls [temp].GetVisibleString ()}");
                    return;
                }

                byte [] xImageBytes = xImageRetrievalResponse.ImageBytes;
                string xImageFilePath = yyPath.Join (yySpecialDirectories.Desktop, $"{xPartialFileName}-{temp + 1}.png");
                File.WriteAllBytes (xImageFilePath, xImageBytes);

                Console.WriteLine ($"Image saved: {xImageFilePath}");
            }
        }
    }
}
