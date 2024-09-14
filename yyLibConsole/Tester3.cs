using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using yyGptLib;
using yyLib;

namespace yyGptLibConsole
{
    public static class Tester3
    {
        // For the user to think how many she/he needs and how much she/he is willing to pay.
        public static void Test (int imageCount)
        {
            var xConnectionInfo = new yyGptImagesConnectionInfo { ApiKey = yyUserSecrets.Default.OpenAi!.ApiKey! };

            // If the AI is faster than usual, the rate limits might be reached.
            for (int temp = 0; temp < imageCount; temp ++)
            {
                var xRequest = new yyGptImagesRequest
                {
                    // Changed to a more general prompt to see the tendency of the results.
                    // I ended up creating multilingual pages in: https://github.com/nao7sep/Resources/tree/main/Static/Beautiful%20People%20and%20Places
                    Prompt = $"A beautiful person in a beautiful place, please.",

                    Model = "dall-e-3",
                    Quality = "hd",
                    Size = "1792x1024"
                };

                if (temp % 2 != 0)
                {
                    xRequest.ResponseFormat = "b64_json";
                    // xRequest.Style = "natural"; doesnt seem to produce good results.
                }

                using (yyGptImagesClient xClient = new (xConnectionInfo))
                {
                    try
                    {
                        var xSendingTask = xClient.SendAsync (xRequest);

                        // Various tasks...

                        xSendingTask.Wait ();

                        var xReadingTask = xClient.ReadToEndAsync ();

                        // Various tasks...

                        xReadingTask.Wait ();

                        string? xJson = xReadingTask.Result;
                        var xResponse1 = yyGptImagesResponseParser.Parse (xJson);

                        if (xSendingTask.Result.HttpResponseMessage.IsSuccessStatusCode)
                        {
                            byte [] xBytes;

                            if (temp % 2 == 0)
                            {
                                using (HttpClient xHttpClient = new ())
                                {
                                    var xResponse2 = xHttpClient.GetAsync (xResponse1.Data! [0].Url).Result;

                                    // Just to make sure.
                                    xResponse2.EnsureSuccessStatusCode ();

                                    xBytes = xResponse2.Content.ReadAsByteArrayAsync ().Result;
                                }
                            }

                            else xBytes = Convert.FromBase64String (xResponse1.Data! [0].B64Json!);

                            // Based on the time when the image becomes available locally.
                            string xFilePathWithoutExtension = yyAppDirectory.MapPath ($"Images{Path.DirectorySeparatorChar}{yyFormatter.ToRoundtripFileNameString (DateTime.UtcNow)}");

                            yyDirectory.CreateParent (xFilePathWithoutExtension);

                            File.WriteAllText (xFilePathWithoutExtension + ".txt", xResponse1.Data [0].RevisedPrompt, Encoding.UTF8);

                            // There seems to be no official document on the format of the image, though.
                            File.WriteAllBytes (xFilePathWithoutExtension + ".png", xBytes);

                            // For content distribution, JPEG is more convenient.

#pragma warning disable CA1416 // Validate platform compatibility
                            using Image xImage = Image.FromFile (xFilePathWithoutExtension + ".png");
                            xImage.Save (xFilePathWithoutExtension + ".jpg", ImageFormat.Jpeg); // Default quality.
#pragma warning restore CA1416

                            Console.WriteLine ("Generated image: " + xFilePathWithoutExtension + ".png");
                        }

                        else
                        {
                            // The following code displays 2 similar portions of text for testing purposes.
                            // The second one should display the string representation of the content of the error model that was successfully deserialized.
                            // Like Tester1.cs, the redundancy is for testing purposes.

                            // If IsSuccessStatusCode is false, refer to the Error property.
                            // It still may be null.
                            // If so, consider referring to the original response string from the server, which must be logged in production code.

                            Console.WriteLine (xJson.GetVisibleString ());

                            Console.WriteLine (JsonSerializer.Serialize (xResponse1, yyJson.DefaultSerializationOptions));
                        }
                    }

                    catch (Exception xException)
                    {
                        yySimpleLogger.Default.TryWriteException (xException);
                        Console.WriteLine (xException.ToString ());
                    }
                }
            }
        }

        public static void GeneratePage (string directoryPath, string summary)
        {
            try
            {
                string xPageTitle = Path.GetFileNameWithoutExtension (directoryPath);

                var xFiles = Directory.EnumerateFiles (directoryPath).Where (x =>
                {
                    // Making sure the .md file is not included to later cause an out-of-range exception.
                    string xExtension = Path.GetExtension (x);

                    return xExtension.Equals (".jpg", StringComparison.OrdinalIgnoreCase) ||
                        xExtension.Equals (".txt", StringComparison.OrdinalIgnoreCase);
                }).
                Order (StringComparer.OrdinalIgnoreCase).ToArray (); // Ordered and finalized.

                string xPageFilePath = Path.Join (directoryPath, $"{xPageTitle}.md");

                StringBuilder xPageFileContents = new ();

                xPageFileContents.AppendLine ($"# {xPageTitle}");
                xPageFileContents.AppendLine ();
                xPageFileContents.AppendLine (summary);

                // Loads the API key from the .yyUserSecrets.json file.
                yyGptChatConnectionInfo xConnectionInfo = new ();

                var xRequest = new yyGptChatRequest ();

                xRequest.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");

                using (yyGptChatClient xClient = new (xConnectionInfo))
                {
                    for (int temp = 0; temp < xFiles.Length; temp += 2)
                    {
                        string xImageFileName = Path.GetFileName (xFiles [temp]),
                            xPrompt = File.ReadAllText (xFiles [temp + 1], Encoding.UTF8).Trim ();

                        xRequest.AddMessage (yyGptChatMessageRole.User, $"Please generate a title without quotation marks or punctuation marks for an image generated with the following prompt: {xPrompt}");

                        var xSendingTask = xClient.SendAsync (xRequest);
                        xSendingTask.Wait ();

                        string? xJson = xClient.ReadToEndAsync ().Result;
                        var xResponse = yyGptChatResponseParser.Parse (xJson);

                        if (xSendingTask.Result.HttpResponseMessage.IsSuccessStatusCode)
                        {
                            // Just to make sure.
                            // Punctuation marks still do appear, but it's not a major problem.
                            string xTitle = xResponse.Choices! [0].Message!.Content.GetVisibleString ().Trim ().Trim ('"').TrimEnd ('.');

                            xRequest.RemoveLastMessage ();

                            xPageFileContents.AppendLine ();
                            xPageFileContents.AppendLine ($"## {xTitle}");
                            xPageFileContents.AppendLine ();
                            xPageFileContents.AppendLine (xPrompt);
                            xPageFileContents.AppendLine ();
                            xPageFileContents.AppendLine ($"![{xTitle}]({xImageFileName})");

                            Console.WriteLine ($"Added to page: {temp / 2 + 1}) {xTitle}");
                        }

                        else
                        {
                            Console.WriteLine (xJson.GetVisibleString ());

                            // Console.WriteLine (JsonSerializer.Serialize (xResponse, yyJson.DefaultSerializationOptions));

                            // No point in continuing.
                            // The file must be failed to be created for the user to notice the problem.
                            return;
                        }
                    }
                }

                File.WriteAllText (xPageFilePath, xPageFileContents.ToString (), Encoding.UTF8);
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }
        }

        public static void TranslatePage (string invariantLanguagePageFilePath, string languageCode, string languageName)
        {
            try
            {
                string xInvariantLanguagePageTitle = Path.GetFileNameWithoutExtension (invariantLanguagePageFilePath),
                    xInvariantLanguagePageFileContents = File.ReadAllText (invariantLanguagePageFilePath, Encoding.UTF8);

                string xNewPageFilePath = Path.Join (Path.GetDirectoryName (invariantLanguagePageFilePath), $"{xInvariantLanguagePageTitle}.{languageCode}.md");

                StringBuilder xNewPageFileContents = new ();

                // Loads the API key from the .yyUserSecrets.json file.
                yyGptChatConnectionInfo xConnectionInfo = new ();

                var xRequest = new yyGptChatRequest ();

                xRequest.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");

                using (yyGptChatClient xClient = new (xConnectionInfo))
                {
                    string? xLastTranslatedImageTitle = null;

                    int xTranslationCount = 0;

                    foreach (string xLine in yyStringLines.EnumerateLines (xInvariantLanguagePageFileContents))
                    {
                        string Translate (string str, int waitingMilliseconds = 3000)
                        {
                            xRequest!.AddMessage (yyGptChatMessageRole.User, $"Please translate the following text into {languageName} and return only the translated text: {str}");

                            var xSendingTask = xClient.SendAsync (xRequest);
                            xSendingTask.Wait ();

                            string? xJson = xClient.ReadToEndAsync ().Result;
                            var xResponse = yyGptChatResponseParser.Parse (xJson);

                            // If I call EnsureSuccessStatusCode here, one exception thrown possibly by a too-many-requests error
                            //     might result in the termination of the whole operation related to the language being translated into.

                            if (xSendingTask.Result.HttpResponseMessage.IsSuccessStatusCode == false)
                            {
                                Thread.Sleep (waitingMilliseconds);

                                // For my own information, when I first Parallel.ForEach-ed the translation operations, I quite often got the too-many-requests error.
                                // But once I implemented the automatic retry mechanism, WITHOUT retrying, the program worked almost fine, leaving only the Chinese language file missing.
                                // Then, I tried translating into Chinese only and it succeeded.
                                // I honestly dont know why Chinese alone failed and dont want to try to reproduce the problem,
                                //     costing myself a lot more money (as my wife is already not very happy knowing I have spent 42 US dollars today :$).
                                // Let's imagine it's relevant to the language's complexity, like one irregular character was contained in one of the responses from the API.
                                // I will fix the problem if it occurs again.

                                // One more thing: My API usage was at 23.59 US dollars when I started the translation into 10 languages.
                                // I succeeded with 9, failed with Chinese, retried Chinese only and succeeded.
                                // Then, the usage was at 42.49 US dollars, indicating translating about 1,100 short titles and 1,100 medium-length prompts would cost us about 20 US dollars.
                                // If these were 2,200 business-related messages that get things done and make a lot more than 20 US dollars, it would be a highly cost-effective tool.

                                Console.BackgroundColor = ConsoleColor.Yellow;
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.WriteLine ("Retrying...");
                                Console.ResetColor ();

                                // Adding 3 seconds each time.
                                // In production code, this value (and the default value) must be configurable.
                                return Translate (str, waitingMilliseconds += 3000);
                            }

                            // During the initial tests, some titles got an unneeded period at the end.
                            string xTranslatedText = xResponse.Choices! [0].Message!.Content.GetVisibleString ().Trim ().Trim ('"').TrimEnd ('.');

                            xRequest.RemoveLastMessage ();

                            return xTranslatedText;
                        }

                        if (xLine.StartsWith ("# "))
                            xNewPageFileContents.AppendLine ($"# {Translate (xLine.Substring (2))}");

                        else if (xLine.StartsWith ("## "))
                        {
                            string xOriginalImageTitle = xLine.Substring (3);
                            xLastTranslatedImageTitle = Translate (xOriginalImageTitle);
                            xNewPageFileContents.AppendLine ($"## {xLastTranslatedImageTitle}");

                            xTranslationCount ++;
                            Console.WriteLine ($"Translated image title: {xTranslationCount}) {xOriginalImageTitle} => {xLastTranslatedImageTitle}");
                        }

                        else if (xLine.StartsWith ("!["))
                            xNewPageFileContents.AppendLine ($"![{xLastTranslatedImageTitle}{xLine.AsSpan (xLine.IndexOf ("]("))}");

                        else if (xLine.Length > 0)
                        {
                            if (xLine.StartsWith ("https://"))
                                xNewPageFileContents.AppendLine (xLine);

                            else xNewPageFileContents.AppendLine (Translate (xLine));
                        }

                        else xNewPageFileContents.AppendLine (xLine);
                    }
                }

                File.WriteAllText (xNewPageFilePath, xNewPageFileContents.ToString (), Encoding.UTF8);
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }
        }
    }
}
