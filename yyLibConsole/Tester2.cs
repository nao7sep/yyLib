using System.Diagnostics;
using System.Text;
using yyGptLib;
using yyLib;

namespace yyGptLibConsole
{
    public static class Tester2
    {
        public static void Test ()
        {
            try
            {
                bool xContinueCounting = true;

                Task xStopwatchTask = Task.Run (() =>
                {
                    Stopwatch xStopwatch = Stopwatch.StartNew ();

                    while (xContinueCounting)
                    {
                        Console.WriteLine ($"Elapsed: {xStopwatch.Elapsed}");
                        Thread.Sleep (100);
                    }
                });

                using yyGptChatConversation xConversation = new (new yyGptChatConnectionInfo { ApiKey = yyUserSecrets.Default.OpenAi!.ApiKey });

                xConversation.Request.N = 3;
                xConversation.Request.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");
                xConversation.Request.AddMessage (yyGptChatMessageRole.User, "A riddle that has multiple answers, please.");

                Task xSendingTask = xConversation.SendAsync ();

                // Various tasks...

                xSendingTask.ContinueWith (x => Console.WriteLine ("Sent.")).Wait ();

                var xReadingTask = xConversation.TryReadAndParseAsync ();

                // Various tasks...

                xReadingTask.ContinueWith (x => Console.WriteLine ("Read.")).Wait ();

                if (xReadingTask.Result.IsSuccess)
                {
                    foreach (string xMessage in xReadingTask.Result.Messages)
                        Console.WriteLine ($"Message: {xMessage.GetVisibleString ()}");

                    xConversation.Request.AddMessage (yyGptChatMessageRole.Assistant, xReadingTask.Result.Messages [Random.Shared.Next (0, xReadingTask.Result.Messages.Count)]);
                }

                else
                {
                    Console.WriteLine ($"RawContent: {xReadingTask.Result.RawContent.GetVisibleString ()}");

                    if (xReadingTask.Result.Messages.Count > 0)
                        Console.WriteLine ($"Error Message: {xReadingTask.Result.Messages [0].GetVisibleString ()}");

                    else Console.WriteLine ($"Exception: {xReadingTask.Result.Exception}");
                }

                // -----------------------------------------------------------------------------

                xConversation.Request.Stream = true;
                xConversation.Request.AddMessage (yyGptChatMessageRole.User, "What is the answer?");

                xSendingTask = xConversation.SendAsync ();

                xSendingTask.ContinueWith (x => Console.WriteLine ("Sent.")).Wait ();

                yyAutoExpandingList <StringBuilder> xBuilders = [];

                while (true)
                {
                    var xChunkReadingTask = xConversation.TryReadAndParseChunkAsync ();

                    if (xChunkReadingTask.Result.IsSuccess)
                    {
                        if (xChunkReadingTask.Result.PartialMessage == null)
                            break;

                        int xIndex = xChunkReadingTask.Result.Index;
                        string? xPartialMessage = xChunkReadingTask.Result.PartialMessage;

                        xBuilders [xIndex].Append (xPartialMessage);

                        Console.WriteLine (FormattableString.Invariant ($"Read Chunk: [{xIndex}] {xPartialMessage.GetVisibleString ()}"));
                    }

                    else
                    {
                        Console.WriteLine ($"RawContent: {xChunkReadingTask.Result.RawContent.GetVisibleString ()}");

                        if (xChunkReadingTask.Result.PartialMessage != null)
                            Console.WriteLine ($"Error Message: {xChunkReadingTask.Result.PartialMessage.GetVisibleString ()}");

                        else Console.WriteLine ($"Exception: {xChunkReadingTask.Result.Exception}");

                        break;
                    }
                }

                foreach (StringBuilder xBuilder in xBuilders)
                    Console.WriteLine ($"Message: {xBuilder.ToString ().GetVisibleString ()}");

                // -----------------------------------------------------------------------------

                xContinueCounting = false;
                xStopwatchTask.Wait ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }
        }
    }
}
