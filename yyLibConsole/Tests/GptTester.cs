using System.Diagnostics.CodeAnalysis;
using System.Text;
using yyLib;

namespace yyLibConsole
{
    internal static class GptTester
    {
        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void Test1 (string firstAssistantDeveloperMessage, string secondAssistantDeveloperMessage, int interactionCount)
        {
            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            yyGptImagesConnectionInfo xImagesConnectionInfo = yyGptImagesConnectionInfo.Default;

            yyGptChatRequest xFirstAssistantRequest = new () { Model = yyGptChat.DefaultModel },
                             xSecondAssistantRequest = new () { Model = yyGptChat.DefaultModel };

            xFirstAssistantRequest.AddDeveloperMessage (firstAssistantDeveloperMessage);
            xSecondAssistantRequest.AddDeveloperMessage (secondAssistantDeveloperMessage);

            xSecondAssistantRequest.Stream = true;

            for (int temp = 0; temp < interactionCount; temp ++)
            {
                var xFirstAssistantResponse = yyGptUtility.GenerateMessagesAsync (xConnectionInfo, xFirstAssistantRequest).Result;

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

                var xSecondAssistantResponse = yyGptUtility.GenerateMessagesChunksAsync (xConnectionInfo, xSecondAssistantRequest,
                    (index, content, cancellationToken) => _OnChunkRetrievedAsync (index, content, cancellationToken)).Result;

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
                        xImagesRequest.ResponseFormat = "b64_json";
                    else xImagesRequest.ResponseFormat = "url";

                    Console.WriteLine ($"Generating image ({xImagesRequest.ResponseFormat})...");

                    var xImagesResponse = yyGptUtility.GenerateImagesAsync (xImagesConnectionInfo, xImagesRequest).Result;

                    if (xImagesResponse.IsSuccess == false)
                    {
                        Console.WriteLine ($"Image generation failed: {(xImagesResponse.Response.Error?.Message).GetVisibleString ()}");
                        break;
                    }

                    string xImagePartialFilePath = yyPath.Join (yySpecialDirectories.Desktop, "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (DateTime.UtcNow)),
                           xImageFilePath = xImagePartialFilePath + ".png",
                           xPromptsFilePath = xImagePartialFilePath + ".txt";

                    File.WriteAllBytes (xImageFilePath, xImagesResponse.ImageBytes [0]);

                    // This code uses AppendLine, which appends a string followed by a newline character sequence.
                    // The newline sequence is determined by the current environment (Environment.NewLine),
                    // making the output platform-dependent (e.g., "\n" on Linux/macOS, "\r\n" on Windows).

                    StringBuilder xPrompts = new ();
                    xPrompts.AppendLine ("[Original Prompt]");
                    xPrompts.AppendLine (xImagesRequest.Prompt);
                    xPrompts.AppendLine ();
                    xPrompts.AppendLine ("[Revised Prompt]");
                    xPrompts.AppendLine (xImagesResponse.RevisedPrompts [0]);

                    File.WriteAllText (xPromptsFilePath, xPrompts.ToString (), yyEncoding.DefaultEncoding);

                    Console.WriteLine ($"Image saved: {xImageFilePath}");
                    Console.WriteLine ($"Prompts saved: {xPromptsFilePath}");
                }
            }
        }
    }
}
