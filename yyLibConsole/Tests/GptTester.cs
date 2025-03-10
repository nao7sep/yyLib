using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using yyLib;

namespace yyLibConsole
{
    internal static class GptTester
    {
        // TestInteractionsOfGptModelsAndGeneratingImages
        //    - Simulates interactions between two GPT assistants by exchanging messages.
        //    - Uses both all-at-once and chunked message generation for varied response handling.
        //    - Periodically triggers image generation based on exchanged messages.
        //    - Retrieves images either as URLs or base64-encoded data and saves them locally.
        //    - Saves the original and revised prompts used for generating images.
        //
        // TestGeneratingMultipleMessagesAndImages
        //    - Generates multiple messages and multiple images, which is its main purpose.
        //    - Uses GPT to generate messages in both batch and chunked modes.
        //    - Demonstrates message retrieval handling with streaming output.
        //    - Requests multiple images based on predefined prompts.
        //    - Downloads and saves multiple generated images using retrieved URLs.
        //
        // TestGeneratingChunkedMessageWithUsage
        //    - Generates a message using a GPT model while streaming the output in chunks.
        //    - Includes usage metadata in the streamed response to track token consumption.
        //    - Handles chunked message retrieval asynchronously, though currently, the retrieval function is a no-op.
        //    - Saves the original request, responses, and usage details as JSON files for further analysis.
        //    - Reports any errors encountered during message generation.
        //
        // TestGeneratingMessagesUsingO1AndO3MiniModels
        //    - Generates messages using different GPT models ("o1" and "o3-mini") at varying levels of reasoning effort (low, medium, high).
        //    - Saves each generated response as a separate JSON file, categorized by model and reasoning effort.
        //    - Demonstrates the impact of different models and reasoning efforts on message generation.
        //    - Reports any errors encountered during message generation.
        //
        // TestUnderstandingImagesUsingVision
        //    - Generates two images based on a given prompt using a GPT-based image model.
        //    - Retrieves the first image via a URL and the second image as a base64-encoded JSON response.
        //    - Saves both images locally for further processing or inspection.
        //    - Uses GPT-4o to analyze the generated images and provide a textual description of their contents.
        //    - Saves the request and response JSONs for reference and debugging purposes.
        //
        // TestGeneratingAndUnderstandingAudio
        //    - Generates an initial audio response from GPT based on a user-provided text prompt.
        //    - Uses the first generated audio as an input to generate a second audio response.
        //    - The second audio response is used as a reference in a third round of message generation, incorporating an additional user prompt.
        //    - Saves all generated audio files as MP3s for further review.
        //    - Extracts and logs expiration details for the second generated audio to track token expiration timelines.
        //    - Saves the final request and response JSONs for debugging and analysis.

        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void TestInteractionsOfGptModelsAndGeneratingImages (string firstAssistantDeveloperMessage, string secondAssistantDeveloperMessage, int interactionCount)
        {
            // -----------------------------------------------------------------------------
            // Preparation
            // -----------------------------------------------------------------------------

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
                // -----------------------------------------------------------------------------
                // All-at-once message generation
                // -----------------------------------------------------------------------------

                var xFirstAssistantResponse = yyGptUtility.GenerateMessagesAsync (xFirstAssistantClient, xFirstAssistantRequest).Result;

                if (xFirstAssistantResponse.IsSuccess == false)
                {
                    Console.WriteLine ($"First Assistant: {(xFirstAssistantResponse.Response.Error?.Message).GetVisibleString ()}");
                    break;
                }

                string xMessage = xFirstAssistantResponse.Messages?.FirstOrDefault () ?? throw new yyUnexpectedNullException ($"First Assistant's Message is null.");
                Console.WriteLine ($"First Assistant: {xMessage.GetVisibleString ()}");

                xFirstAssistantRequest.AddAssistantMessage (xMessage);
                xSecondAssistantRequest.AddUserMessage (xMessage);

                // -----------------------------------------------------------------------------
                // Chunked message generation
                // -----------------------------------------------------------------------------

                static Task _OnChunkRetrievedAsync (int index, string? content, CancellationToken cancellationToken)
                {
                    Console.Write (content);
                    return Task.CompletedTask;
                }

                Console.Write ("Second Assistant: ");

                // Refer to: https://github.com/nao7sep/Resources/blob/main/Documents/AI-Generated%20Notes/Understanding%20Async%20Lambdas%20and%20Task.CompletedTask%20in%20C%23.md
                // for more information about the async lambda syntax.

                var xSecondAssistantResponse = yyGptUtility.GenerateMessagesChunksAsync (xSecondAssistantClient, xSecondAssistantRequest,
                    async (index, content, cancellationToken) => await _OnChunkRetrievedAsync (index, content, cancellationToken).ConfigureAwait (false)).Result;

                if (xSecondAssistantResponse.IsSuccess == false)
                {
                    Console.WriteLine ((xSecondAssistantResponse.Responses.FirstOrDefault ()?.Error?.Message).GetVisibleString ());
                    break;
                }

                Console.WriteLine ();

                xMessage = xSecondAssistantResponse.Messages?.FirstOrDefault () ?? throw new yyUnexpectedNullException ($"Second Assistant's Message is null.");

                xFirstAssistantRequest.AddUserMessage (xMessage);
                xSecondAssistantRequest.AddAssistantMessage (xMessage);

                if ((temp + 1) % 5 == 0) // 4, 9, 14...
                {
                    // -----------------------------------------------------------------------------
                    // Image generation
                    // -----------------------------------------------------------------------------

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

                    // -----------------------------------------------------------------------------
                    // Image retrieval
                    // -----------------------------------------------------------------------------

                    byte [] xImageBytes = []; // Single image.

                    if (xImagesRequest.ResponseFormat.Equals ("url", StringComparison.OrdinalIgnoreCase))
                    {
                        string xUrl = xImagesResponse.Urls?.FirstOrDefault () ?? throw new yyUnexpectedNullException ($"Image URL is null.");
                        var xImageRetrievalResponse = yyGptUtility.RetrieveImageBytesAsync (xHttpClient, xUrl).Result;

                        if (xImageRetrievalResponse.IsSuccess == false)
                        {
                            Console.WriteLine ($"Image retrieval failed: {xUrl.GetVisibleString ()}");
                            break;
                        }

                        xImageBytes = xImageRetrievalResponse.ImageBytes ?? throw new yyUnexpectedNullException ($"Image Bytes is null.");
                    }

                    else xImageBytes = xImagesResponse.ImagesBytes?.FirstOrDefault () ?? throw new yyUnexpectedNullException ($"Image Bytes is null.");

                    string xImagePartialFilePath = yyPath.Join (yySpecialDirectories.Desktop, "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (DateTime.UtcNow)),
                           xImageFilePath = xImagePartialFilePath + ".png",
                           xPromptsFilePath = xImagePartialFilePath + ".txt";

                    File.WriteAllBytes (xImageFilePath, xImageBytes);
                    Console.WriteLine ($"Image saved: {xImageFilePath}");

                    // -----------------------------------------------------------------------------
                    // Save image prompts
                    // -----------------------------------------------------------------------------

                    // This code uses AppendLine, which appends a string followed by a newline character sequence.
                    // The newline sequence is determined by the current environment (Environment.NewLine),
                    // making the output platform-dependent (e.g., "\n" on Linux/macOS, "\r\n" on Windows).

                    StringBuilder xPrompts = new ();
                    xPrompts.AppendLine ("[Original Prompt]");
                    xPrompts.AppendLine (xImagesRequest.Prompt);
                    xPrompts.AppendLine ();
                    xPrompts.AppendLine ("[Revised Prompt]");
                    xPrompts.AppendLine (xImagesResponse.RevisedPrompts?.FirstOrDefault ().GetVisibleString ());

                    File.WriteAllText (xPromptsFilePath, xPrompts.ToString (), yyEncoding.DefaultEncoding);
                    Console.WriteLine ($"Prompts saved: {xPromptsFilePath}");
                }
            }
        }

        public static void TestGeneratingMultipleMessagesAndImages (int messageCount, int imageCount)
        {
            // -----------------------------------------------------------------------------
            // Multiple and all-at-once message generation
            // -----------------------------------------------------------------------------

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
                Console.WriteLine ($"Message {(temp + 1).ToString (CultureInfo.InvariantCulture)}: {xFirstResponse.Messages?.ElementAtOrDefault (temp).GetVisibleString ()}");

            // -----------------------------------------------------------------------------
            // Multiple and chunked message generation
            // -----------------------------------------------------------------------------

            xRequest.Stream = true;

            static Task _OnChunkRetrievedAsync (int index, string? content, CancellationToken cancellationToken)
            {
                // Demonstrates how whitespace characters appear in the message chunks.
                Console.WriteLine ($"Message {(index + 1).ToString (CultureInfo.InvariantCulture)}: {content}⏎"); // U+23CE: Return Symbol.
                return Task.CompletedTask;
            }

            // The following redundant lambda is for demonstration purposes.
            // Refer to: https://github.com/nao7sep/Resources/blob/main/Documents/AI-Generated%20Notes/Understanding%20Async%20Lambdas%20and%20Task.CompletedTask%20in%20C%23.md
            // for more information about the async lambda syntax.

            var xSecondResponse = yyGptUtility.GenerateMessagesChunksAsync (xClient, xRequest,
                async (index, content, cancellationToken) => await _OnChunkRetrievedAsync (index, content, cancellationToken).ConfigureAwait (false)).Result;

            if (xSecondResponse.IsSuccess == false)
            {
                Console.WriteLine ((xSecondResponse.Responses?.FirstOrDefault ()?.Error?.Message).GetVisibleString ());
                return;
            }

            for (int temp = 0; temp < messageCount; temp ++)
                Console.WriteLine ($"Message {(temp + 1).ToString (CultureInfo.InvariantCulture)}: {xSecondResponse.Messages?.ElementAtOrDefault (temp).GetVisibleString ()}");

            // -----------------------------------------------------------------------------
            // Multiple image generation
            // -----------------------------------------------------------------------------

            yyGptImagesConnectionInfo xImagesConnectionInfo = yyGptImagesConnectionInfo.Default;
            using var xImagesClient = new yyGptImagesClient (xImagesConnectionInfo);
            using var xHttpClient = yyGptUtility.CreateImageRetrievalHttpClient (xImagesConnectionInfo);

            yyGptImagesRequest xImagesRequest = new ()
            {
                // Quality and Style are not supported by DALL-E 2 model.
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

            // -----------------------------------------------------------------------------
            // Multiple image retrieval
            // -----------------------------------------------------------------------------

            // Time when images generation is completed.
            DateTime xUtcNow = DateTime.UtcNow;
            string xPartialFileName = "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (xUtcNow);

            for (int temp = 0; temp < imageCount; temp ++)
            {
                string xUrl = xImagesResponse.Urls?.ElementAtOrDefault (temp) ?? throw new yyUnexpectedNullException ($"Image URL is null.");
                var xImageRetrievalResponse = yyGptUtility.RetrieveImageBytesAsync (xHttpClient, xUrl).Result;

                if (xImageRetrievalResponse.IsSuccess == false)
                {
                    Console.WriteLine ($"Image retrieval failed: {xUrl.GetVisibleString ()}");
                    return;
                }

                byte [] xImageBytes = xImageRetrievalResponse.ImageBytes ?? throw new yyUnexpectedNullException ($"Image Bytes is null.");
                string xImageFilePath = yyPath.Join (yySpecialDirectories.Desktop, $"{xPartialFileName}-{(temp + 1).ToString (CultureInfo.InvariantCulture)}.png");
                File.WriteAllBytes (xImageFilePath, xImageBytes);
                Console.WriteLine ($"Image saved: {xImageFilePath}");
            }
        }

        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void TestGeneratingChunkedMessageWithUsage (string prompt)
        {
            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            using var xClient = new yyGptChatClient (xConnectionInfo);

            yyGptChatRequest xRequest = new ()
            {
                Model = yyGptChat.DefaultModel,
                Stream = true,

                StreamOptions = new ()
                {
                    IncludeUsage = true
                }
            };

            xRequest.AddUserMessage (prompt);

            Console.Write ("Generating message...");

            static Task _OnChunkRetrievedAsync (int index, string? content, CancellationToken cancellationToken) => Task.CompletedTask;

            var xResponse = yyGptUtility.GenerateMessagesChunksAsync (xClient, xRequest,
                async (index, content, cancellationToken) => await _OnChunkRetrievedAsync (index, content, cancellationToken).ConfigureAwait (false)).Result;

            Console.WriteLine ();

            if (xResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Message generation failed: {(xResponse.Responses?.FirstOrDefault ()?.Error?.Message).GetVisibleString ()}");
                return;
            }

            DateTime xUtcNow = DateTime.UtcNow;

            string xPartialFileName = "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (xUtcNow),
                   xRequestFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-Request.json"),
                   xResponsesFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-Responses.json"),
                   xUsageFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-Usage.json");

            File.WriteAllText (xRequestFilePath, xResponse.RequestJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Request saved: {xRequestFilePath}");

            string xResponsesJsonString = JsonSerializer.Serialize (xResponse.Responses, yyJson.DefaultSerializationOptions);
            File.WriteAllText (xResponsesFilePath, xResponsesJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Responses saved: {xResponsesFilePath}");

            // May seem redundant.
            // Just offering a little easier way to refer to the usage object.
            // Expected to clarify that the whole point of this test method is the usage object.

            string xUsageJsonString = JsonSerializer.Serialize (xResponse.Responses.Last ().Usage, yyJson.DefaultSerializationOptions);
            File.WriteAllText (xUsageFilePath, xUsageJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Usage saved: {xUsageFilePath}");
        }

        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void TestGeneratingMessagesUsingO1AndO3MiniModels (string prompt)
        {
            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            using var xClient = new yyGptChatClient (xConnectionInfo);

            DateTime xUtcNow = DateTime.UtcNow;
            string xPartialFileName = "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (xUtcNow);

            void _GenerateAndSaveMessage (string model, string reasoningEffort)
            {
                yyGptChatRequest xRequest = new ()
                {
                    Model = model,
                    ReasoningEffort = reasoningEffort
                };

                xRequest.AddUserMessage (prompt);

                Console.Write ($"Generating message ({model}, {reasoningEffort})...");

                var xResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

                Console.WriteLine ();

                if (xResponse.IsSuccess == false)
                {
                    Console.WriteLine ($"Message generation failed: {(xResponse.Response.Error?.Message).GetVisibleString ()}");
                    return;
                }

                string xJsonFileName = $"{xPartialFileName}-{model}-{reasoningEffort}.json",
                       xJsonFilePath = yyPath.Join (yySpecialDirectories.Desktop, xJsonFileName),
                       xJsonString = JsonSerializer.Serialize (xResponse.Response, yyJson.DefaultSerializationOptions); // Parsed and formatted.

                File.WriteAllText (xJsonFilePath, xJsonString, yyEncoding.DefaultEncoding);
                Console.WriteLine ($"Response saved: {xJsonFilePath}");
            }

            _GenerateAndSaveMessage ("o1", "low");
            _GenerateAndSaveMessage ("o1", "medium");
            _GenerateAndSaveMessage ("o1", "high");

            _GenerateAndSaveMessage ("o3-mini", "low");
            _GenerateAndSaveMessage ("o3-mini", "medium");
            _GenerateAndSaveMessage ("o3-mini", "high");
        }

        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void TestUnderstandingImagesUsingVision (string imagesGenerationPrompt)
        {
            // -----------------------------------------------------------------------------
            // Generate images to understand
            // -----------------------------------------------------------------------------

            yyGptImagesConnectionInfo xImagesConnectionInfo = yyGptImagesConnectionInfo.Default;
            using var xImagesClient = new yyGptImagesClient (xImagesConnectionInfo);
            using var xHttpClient = yyGptUtility.CreateImageRetrievalHttpClient (xImagesConnectionInfo);

            yyGptImagesRequest xImagesRequest = new ()
            {
                Prompt = imagesGenerationPrompt,
                Model = yyGptImages.DefaultModel,
                Quality = yyGptImages.DefaultQuality,
                Size = yyGptImages.DefaultSize,
                Style = yyGptImages.DefaultStyle,
                ResponseFormat = default // For the compiler NOT to merge the following code.
            };

            DateTime xUtcNow = DateTime.UtcNow;

            string xPartialFileName = "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (xUtcNow),
                   xFirstImageFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-First.png"),
                   xSecondImageFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-Second.png");

            xImagesRequest.ResponseFormat = "url";

            Console.Write ("Generating first image...");

            var xImagesResponse = yyGptUtility.GenerateImagesAsync (xImagesClient, xImagesRequest).Result;

            Console.WriteLine ();

            if (xImagesResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Image generation failed: {(xImagesResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            string xImageUrl = xImagesResponse.Urls?.FirstOrDefault () ?? throw new yyUnexpectedNullException ($"Image URL is null.");

            var xImageRetrievalResponse = yyGptUtility.RetrieveImageBytesAsync (xHttpClient, xImageUrl).Result;

            if (xImageRetrievalResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Image retrieval failed: {xImageUrl.GetVisibleString ()}");
                return;
            }

            byte [] xFirstImageBytes = xImageRetrievalResponse.ImageBytes ?? throw new yyUnexpectedNullException ($"Image Bytes is null.");

            File.WriteAllBytes (xFirstImageFilePath, xFirstImageBytes);
            Console.WriteLine ($"First image saved: {xFirstImageFilePath}");

            xImagesRequest.ResponseFormat = "b64_json";

            Console.Write ("Generating second image...");

            xImagesResponse = yyGptUtility.GenerateImagesAsync (xImagesClient, xImagesRequest).Result;

            Console.WriteLine ();

            if (xImagesResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Image generation failed: {(xImagesResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            byte [] xImageBytes = xImagesResponse.ImagesBytes?.FirstOrDefault () ?? throw new yyUnexpectedNullException ($"Image Bytes is null.");

            File.WriteAllBytes (xSecondImageFilePath, xImageBytes);
            Console.WriteLine ($"Second image saved: {xSecondImageFilePath}");

            // -----------------------------------------------------------------------------
            // Understand images
            // -----------------------------------------------------------------------------

            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            using var xClient = new yyGptChatClient (xConnectionInfo);

            yyGptChatRequest xRequest = new ()
            {
                // 2025-02-28: Currently, models that can take images as input include o1, gpt-4o, gpt-4o-mini, and gpt-4-turbo.
                // https://platform.openai.com/docs/guides/vision/vision
                Model = "gpt-4o"
            };

            xRequest.AddUserMessage (
            [
                new yyGptChatContentPart
                {
                    Type = "text",
                    Text = "What do you see in these images?"
                },

                new yyGptChatContentPart
                {
                    Type = "image_url",

                    ImageUrl = new yyGptChatImage
                    {
                        Url = xImageUrl
                    }
                },

                new yyGptChatContentPart
                {
                    Type = "image_url",

                    ImageUrl = new yyGptChatImage
                    {
                        Url = yyGptUtility.BytesToUrlProperty ("image/png", xImageBytes)
                    }
                }
            ]);

            Console.Write ("Understanding images...");

            var xResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

            Console.WriteLine ();

            if (xResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Images understanding failed: {(xResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            string xRequestFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-Request.json"),
                   xResponseFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-Response.json");

            File.WriteAllText (xRequestFilePath, xResponse.RequestJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Request saved: {xRequestFilePath}");

            string xResponseJsonString = JsonSerializer.Serialize (xResponse.Response, yyJson.DefaultSerializationOptions);
            File.WriteAllText (xResponseFilePath, xResponseJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Response saved: {xResponseFilePath}");
        }

        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        public static void TestGeneratingAndUnderstandingAudio (string modelName, string firstPrompt, string secondPrompt)
        {
            // -----------------------------------------------------------------------------
            // Preparation
            // -----------------------------------------------------------------------------

            yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
            using var xClient = new yyGptChatClient (xConnectionInfo);

            yyGptChatRequest xRequest = new ()
            {
                Model = modelName,

                Modalities =
                [
                    "text",
                    "audio"
                ],

                Audio = new ()
                {
                    Voice = "sage",
                    Format = "mp3"
                }
            };

            xRequest.AddUserMessage (firstPrompt);

            DateTime xUtcNow = DateTime.UtcNow;

            string xPartialFileName = "GptTest-" + yyConverter.DateTimeToRoundtripFileNameString (xUtcNow),
                   xFirstAudioFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-FirstAudio.mp3"),
                   xSecondAudioFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-SecondAudio.mp3"),
                   xThirdAudioFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-ThirdAudio.mp3"),
                   xLastRequestFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-LastRequest.json"),
                   xLastResponseFilePath = yyPath.Join (yySpecialDirectories.Desktop, xPartialFileName + "-LastResponse.json");

            // -----------------------------------------------------------------------------
            // Generate first audio
            // -----------------------------------------------------------------------------

            Console.Write ("Generating first audio...");

            var xFirstResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

            Console.WriteLine ();

            if (xFirstResponse.IsSuccess == false)
            {
                Console.WriteLine ($"First audio generation failed: {(xFirstResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            string xFirstAudioData = xFirstResponse.Response.Choices?.FirstOrDefault ()?.Message?.Audio?.Data ?? throw new yyUnexpectedNullException ("First Audio Data is null.");
            byte [] xFirstAudioBytes = yyGptUtility.DataPropertyToBytes (xFirstAudioData);
            File.WriteAllBytes (xFirstAudioFilePath, xFirstAudioBytes);
            Console.WriteLine ($"First audio saved: {xFirstAudioFilePath}");

            // -----------------------------------------------------------------------------
            // Generate second audio
            // -----------------------------------------------------------------------------

            // Resetting the request with the generated audio as the first user message.

            xRequest.Messages.Clear ();

            xRequest.AddUserMessage (
            [
                new yyGptChatContentPart
                {
                    Type = "input_audio",

                    InputAudio = new yyGptChatAudio
                    {
                        Data = xFirstAudioData,
                        Format = "mp3"
                    }
                }
            ]);

            Console.Write ("Generating second audio...");

            var xSecondResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

            Console.WriteLine ();

            if (xSecondResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Second audio generation failed: {(xSecondResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            yyGptChatAudio xSecondAudio = xSecondResponse.Response.Choices?.FirstOrDefault ()?.Message?.Audio ?? throw new yyUnexpectedNullException ($"Second Audio is null.");

            string xSecondAudioId = xSecondAudio.Id ?? throw new yyUnexpectedNullException ($"Second Audio ID is null."),
                   xSecondAudioData = xSecondAudio.Data ?? throw new yyUnexpectedNullException ($"Second Audio Data is null.");

            byte [] xSecondAudioBytes = yyGptUtility.DataPropertyToBytes (xSecondAudioData);
            File.WriteAllBytes (xSecondAudioFilePath, xSecondAudioBytes);
            Console.WriteLine ($"Second audio saved: {xSecondAudioFilePath}");

            int xExpiresAtValue = xSecondAudio.ExpiresAt ?? throw new yyUnexpectedNullException ($"Second Audio ExpiresAt is null.");
            DateTime xExpiresAtUtc = yyGptUtility.ExpiresAtPropertyToUtc (xExpiresAtValue);
            Console.WriteLine ($"Second audio expires at: {yyConverter.DateTimeToRoundtripString (xExpiresAtUtc)} ({(xExpiresAtUtc - DateTime.UtcNow).TotalSeconds} seconds from now)");

            // -----------------------------------------------------------------------------
            // Generate third audio
            // -----------------------------------------------------------------------------

            xRequest.Messages.Add (new ()
            {
                Role = yyGptChatRole.Assistant,

                Audio = new ()
                {
                    Id = xSecondAudioId
                }
            });

            xRequest.AddUserMessage (secondPrompt);

            Console.Write ("Generating third audio...");

            var xThirdResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

            Console.WriteLine ();

            if (xThirdResponse.IsSuccess == false)
            {
                Console.WriteLine ($"Third audio generation failed: {(xThirdResponse.Response.Error?.Message).GetVisibleString ()}");
                return;
            }

            string xThirdAudioData = xThirdResponse.Response.Choices?.FirstOrDefault ()?.Message?.Audio?.Data ?? throw new yyUnexpectedNullException ($"Third Audio Data is null.");
            byte [] xThirdAudioBytes = yyGptUtility.DataPropertyToBytes (xThirdAudioData);
            File.WriteAllBytes (xThirdAudioFilePath, xThirdAudioBytes);
            Console.WriteLine ($"Third audio saved: {xThirdAudioFilePath}");

            // -----------------------------------------------------------------------------
            // Save last request and response
            // -----------------------------------------------------------------------------

            File.WriteAllText (xLastRequestFilePath, xThirdResponse.RequestJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Last request saved: {xLastRequestFilePath}");

            string xLastResponseJsonString = JsonSerializer.Serialize (xThirdResponse.Response, yyJson.DefaultSerializationOptions);
            File.WriteAllText (xLastResponseFilePath, xLastResponseJsonString, yyEncoding.DefaultEncoding);
            Console.WriteLine ($"Last response saved: {xLastResponseFilePath}");
        }
    }
}
