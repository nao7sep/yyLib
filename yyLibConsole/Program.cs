using System.Diagnostics.CodeAnalysis;
using yyLib;

namespace yyLibConsole
{
    // Marks the Program class as 'internal sealed':
    // - 'internal': Restricts access to the current assembly, ensuring the class is not exposed to external projects.
    // - 'sealed': Prevents other classes from inheriting from Program, as it serves a single, specific purpose.
    // This change improves encapsulation, aligns with best practices for entry-point classes, and slightly optimizes performance.
    internal sealed class Program
    {
        // Suppresses the warning about catching general exceptions (CA1031).
        [SuppressMessage ("Design", "CA1031")]
        // Suppresses the warning about passing literals or constant strings as parameters for methods expecting localized resources (CA1303).
        [SuppressMessage ("Globalization", "CA1303")]
        // Suppresses the warning for unused method parameters.
        [SuppressMessage ("Style", "IDE0060")]
        private static void Main (string [] args)
        {
            try
            {
                // API key must be specified in user secrets or app settings:

                // GptTester.TestInteractionsOfGptModelsAndGeneratingImages (
                //     firstAssistantDeveloperMessage: "You always say a random word.",
                //     secondAssistantDeveloperMessage: "You always make a random sentence using the provided word.",
                //     interactionCount: 30);

                // GptTester.TestGeneratingMultipleMessagesAndImages (messageCount: 3, imageCount: 3);
                // GptTester.TestGeneratingChunkedMessageWithUsage ("Generate a short story.");
                // GptTester.TestGeneratingMessagesUsingO1AndO3MiniModels ("Generate a question you cannot answer and try answering it.");
                // GptTester.TestUnderstandingImagesUsingVision ("Please generate an image of something beautiful.");
                // GptTester.TestGeneratingAndUnderstandingAudio ("gpt-4o-audio-preview", "Generate a message that would receive a short response.", "Summarize this conversation.");

                // JsonTester.TestParsingAndDisplayingJsonFile ("JSON-Edge-Cases.json");

                // Set "from" and "to" email addresses for testing.
                // Default connection must be specified in user secrets or app settings.
                // MailTester.TestSendingMessageAndSavingJsonAndMimeRepresentations (from: string.Empty, to: string.Empty);

                Gaimen.Run ();
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }

            finally
            {
                Console.Write ("Press any key to exit: ");
                Console.ReadKey (true);
                Console.WriteLine ();
            }
        }
    }
}
