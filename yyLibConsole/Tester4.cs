using System.Text;
using System.Text.Json;
using yyGptLib;
using yyLib;

namespace yyGptLibConsole
{
    public static class Tester4
    {
        public static void SaveJsonStrings ()
        {
            try
            {
                string xOutputDirectoryPath = @"C:\Repositories\Resources\Misc\yyGptLibConsole_Tester4";

                if (Directory.Exists (xOutputDirectoryPath))
                    Directory.Delete (xOutputDirectoryPath, recursive: true);

                yyDirectory.Create (xOutputDirectoryPath);

                void _SaveJsonString (string key, string str)
                {
                    string xFilePath = Path.Join (xOutputDirectoryPath, $"{key}.json");
                    File.WriteAllText (xFilePath, str, Encoding.UTF8);
                }

                void _SaveData (string key, object data)
                {
                    string xJsonString = JsonSerializer.Serialize (data, yyJson.DefaultSerializationOptions);
                    _SaveJsonString (key, xJsonString);
                }

                yyGptChatRequest xChatRequest = new yyGptChatRequest
                {
                    // A lot of the following values have little meaning.
                    // Often, they are just a little more or less than the default values.
                    // The rest are by GitHub Copilot.

                    Messages =
                    [
                        new yyGptChatMessage
                        {
                            Role = yyGptChatMessageRole.System,
                            Content = "You are a helpful assistant.",
                            Name = "System"
                        },

                        new yyGptChatMessage
                        {
                            Role = yyGptChatMessageRole.User,
                            // https://platform.openai.com/docs/api-reference/chat/create#chat-create-response_format
                            Content = "What is the capital of France? Respond in JSON format.",
                            Name = "User"
                        },
                    ],

                    Model = "gpt-4-turbo",

                    FrequencyPenalty = 0.1,

                    LogitBias = new Dictionary <int, double>
                    {
                        { 50256, 1.0 }
                    },

                    LogProbs = true,
                    TopLogProbs = 10,
                    MaxTokens = 100,
                    N = 2,
                    PresencePenalty = 0.1,

                    ResponseFormat = new yyGptChatRequestResponseFormat
                    {
                        Type = "json_object"
                    },

                    Seed = Random.Shared.Next (),
                    Stop = ["world"],
                    Stream = false,
                    Temperature = 1.1,
                    TopP = 0.9,
                    User = "yyGptLibConsole_Tester4"
                };

                _SaveData ("ChatRequest", xChatRequest);

                yyGptChatConnectionInfo xChatConnectionInfo = new yyGptChatConnectionInfo
                {
                    // Everything is loaded from the settings file.
                    // I wont keep the file, but the content was checked.
                };

                // _SaveData ("ChatConnectionInfo", xChatConnectionInfo);

                using (yyGptChatClient xChatClient = new (xChatConnectionInfo))
                {
                    var xChatSendingTask = xChatClient.SendAsync (xChatRequest);
                    xChatSendingTask.Wait ();

                    var xChatReadingTask = xChatClient.ReadToEndAsync ();
                    xChatReadingTask.Wait ();

                    string? xChatJsonString = xChatReadingTask.Result;
                    _SaveJsonString ("ChatResponse", xChatJsonString!);

                    var xChatResponse = yyGptChatResponseParser.Parse (xChatJsonString);
                    _SaveData ("ChatParsedResponse", xChatResponse);
                }

                // The chunk transfer mode and image-related things are not tested.
                // The primary purpose of this test is to test things related to logprobs.
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }
        }
    }
}
