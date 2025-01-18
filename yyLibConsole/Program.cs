﻿using System.Diagnostics.CodeAnalysis;
using yyLib;

namespace yyLibConsole
{
    // Marks the Program class as 'internal sealed':
    // - 'internal': Restricts access to the current assembly, ensuring the class is not exposed to external projects.
    // - 'sealed': Prevents other classes from inheriting from Program, as it serves a single, specific purpose.
    // This change improves encapsulation, aligns with best practices for entry-point classes, and slightly optimizes performance.
    internal sealed class Program
    {
        // Suppresses the warning for unused method parameters.
        [SuppressMessage ("Style", "IDE0060")]
        static void Main (string [] args)
        {
            try
            {
                // API key must be specified in user secrets or app settings.
                // GptTest.Run (
                //     firstAssistantSystemMessage: "You always say a random word.",
                //     secondAssistantSystemMessage: "You always make a random sentence using the provided word.",
                //     interactionCount: 10);

                // JsonTest.Run ();

                // Set "from" and "to" email addresses for testing.
                // Default connection must be specified in user secrets or app settings.
                // MailTest.Run (from: string.Empty, to: string.Empty);
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
