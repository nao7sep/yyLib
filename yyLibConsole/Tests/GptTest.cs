using System.Text;
using yyLib;

namespace yyLibConsole
{
    public static class GptTest
    {
        public static void Run (string firstAssistantSystemMessage, string secondAssistantSystemMessage, int interactionCount)
        {
            yyGptChatConnectionInfo xConnectionInfo = new ();
            yyGptImagesConnectionInfo xImagesConnectionInfo = new ();

            yyGptChatRequest xFirstAssistantRequest = new (),
                            xSecondAssistantRequest = new ();

            xFirstAssistantRequest.AddSystemMessage (firstAssistantSystemMessage);
            xSecondAssistantRequest.AddSystemMessage (secondAssistantSystemMessage);

            for (int temp = 0; temp < interactionCount; temp ++)
            {
                var xFirstAssistantResponse = yyGptUtility.GenerateMessagesAsync (xConnectionInfo, xFirstAssistantRequest).Result;

                string xGeneratedMessage = xFirstAssistantResponse.GeneratedMessages! [0];
                Console.WriteLine ($"First Assistant: {xGeneratedMessage}");

                xFirstAssistantRequest.AddAssistantMessage (xGeneratedMessage);
                xSecondAssistantRequest.AddUserMessage (xGeneratedMessage);

                Task _OnChunkRetrievedAsync (int index, string content, CancellationToken cancellationToken)
                {
                    Console.Write (content);
                    return Task.CompletedTask;
                }

                Console.Write ("Second Assistant: ");

                var xSecondAssistantResponse = yyGptUtility.GenerateMessagesChunksAsync (xConnectionInfo, xSecondAssistantRequest,
                    (index, content, cancellationToken) => _OnChunkRetrievedAsync (index, content, cancellationToken)).Result;

                Console.WriteLine ();

                xGeneratedMessage = xSecondAssistantResponse.GeneratedMessages! [0];

                xFirstAssistantRequest.AddUserMessage (xGeneratedMessage);
                xSecondAssistantRequest.AddAssistantMessage (xGeneratedMessage);

                if ((temp + 1) % 5 == 0) // 4, 9, 14...
                {
                    yyGptImagesRequest xImagesRequest = new ()
                    {
                        // https://platform.openai.com/docs/guides/images

                        Prompt = xGeneratedMessage,
                        Model = "dall-e-3",
                        Quality = "hd", // or "standard"
                        Size = "1024x1024" // or "1792x1024" or "1024x1792"
                    };

                    if ((temp + 6) % 10 == 0) // 4, 14...
                        xImagesRequest.ResponseFormat = "b64_json";
                    else xImagesRequest.ResponseFormat = "url";

                    Console.WriteLine ($"Generating image ({xImagesRequest.ResponseFormat})...");

                    var xImagesResponse = yyGptUtility.GenerateImagesAsync (xImagesConnectionInfo, xImagesRequest).Result;

                    string xImagePartialFilePath = yyPath.Join (yySpecialDirectories.Desktop, "GptTest-" + yyConvertor.DateTimeToRoundtripFileNameString (DateTime.UtcNow)),
                           xImageFilePath = xImagePartialFilePath + ".png",
                           xPromptsFilePath = xImagePartialFilePath + ".txt";

                    File.WriteAllBytes (xImageFilePath, xImagesResponse.ImageBytes! [0]);

                    StringBuilder xPrompts = new ();
                    xPrompts.AppendLine ("[Original Prompt]");
                    xPrompts.AppendLine (xImagesRequest.Prompt);
                    xPrompts.AppendLine ();
                    xPrompts.AppendLine ("Revised Prompt]");
                    xPrompts.AppendLine (xImagesResponse.RevisedPrompts! [0]);

                    File.WriteAllText (xPromptsFilePath, xPrompts.ToString (), Encoding.UTF8);

                    Console.WriteLine ($"Image saved: {xImageFilePath}");
                    Console.WriteLine ($"Prompts saved: {xPromptsFilePath}");
                }
            }
        }
    }
}
