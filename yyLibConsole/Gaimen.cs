using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using yyLib;

// Suppresses the CA1050 warning, which advises against declaring non-namespaced types in the global namespace.
[SuppressMessage ("Design", "CA1050")]
public static class Gaimen
{
    public static void Run ()
    {
        yyGptChatConnectionInfo xConnectionInfo = yyGptChatConnectionInfo.Default;
        xConnectionInfo.Timeout *= 10; // 300 seconds.
        using var xClient = new yyGptChatClient (xConnectionInfo);

        yyGptChatRequest xRequest = new ()
        {
            Model = "gpt-4o"
        };

        xRequest.AddDeveloperMessage (string.Join (" ",
        [
            "Transcribe provided images.",
            "Most contents will be in Japanese.",
            "When you detect contents that are in English and are seemingly translations of Japanese contents, ignore the English contents.",
            "Consider each image as a part of a document, meaning you will use moderate headings.",
            "If the first image of a series is specified as a front page, you can use larger headings.",
        ]));

        (string ResponseJsonString, string Transcription) _TranscribeImages (string directoryPath, IEnumerable <string> fileNames, string? prompt)
        {
            try
            {
                List <yyGptChatContentPart> xContentParts = [];

                if (prompt != null)
                {
                    xContentParts.Add (new ()
                    {
                        Type = "text",
                        Text = prompt
                    });
                }

                foreach (string xFileName in fileNames)
                {
                    string xFilePath = yyPath.Join (directoryPath, xFileName);
                    byte [] xImageBytes = File.ReadAllBytes (xFilePath);

                    xContentParts.Add (new ()
                    {
                        Type = "image_url",

                        ImageUrl = new yyGptChatImage
                        {
                            Url = yyGptUtility.BytesToUrlProperty ("image/jpeg", xImageBytes)
                        }
                    });
                }

                xRequest.AddUserMessage (xContentParts);

                var xResponse = yyGptUtility.GenerateMessagesAsync (xClient, xRequest).Result;

                if (xResponse.IsSuccess == false)
                    throw new yyException ((xResponse.Response.Error?.Message).GetVisibleString ());

                string xTranscription = xResponse.Messages?.FirstOrDefault () ?? throw new yyUnexpectedNullException ("Transcription is missing.");
                return (xResponse.ResponseJsonString, xTranscription);
            }

            finally
            {
                while (xRequest.Messages.Count > 1)
                    xRequest.Messages.RemoveAt (xRequest.Messages.Count - 1);
            }
        }

        string _GetSimplifiedFileNameWithoutExtension (string fileName)
        {
            string xFileNameWithoutExtension = Path.GetFileNameWithoutExtension (fileName);
            int xIndex = xFileNameWithoutExtension.IndexOf ('_');

            if (xIndex < 0)
                return xFileNameWithoutExtension;

            string xTimestampString = xFileNameWithoutExtension.Substring (0, xIndex);
            long xTimestamp = long.Parse (xTimestampString, CultureInfo.InvariantCulture);

            string xNumberString = xFileNameWithoutExtension.Substring (xIndex + 1);
            int xNumber = int.Parse (xNumberString, CultureInfo.InvariantCulture);

            xTimestamp += xNumber;

            return xTimestamp.ToString (CultureInfo.InvariantCulture);
        }

        string _Translate (string str, string language)
        {
            yyGptChatRequest xTranslationRequest = new ()
            {
                Model = "gpt-4o"
            };

            xTranslationRequest.AddDeveloperMessage ($"Translate provided text to {language} and return only the translated text.");
            xTranslationRequest.AddUserMessage (str);

            var xTranslationResponse = yyGptUtility.GenerateMessagesAsync (xClient, xTranslationRequest).Result;

            if (xTranslationResponse.IsSuccess == false)
                throw new yyException ((xTranslationResponse.Response.Error?.Message).GetVisibleString ());

            return xTranslationResponse.Messages?.FirstOrDefault () ?? throw new yyUnexpectedNullException ("Translation is missing.");
        }

        int _TranslateIfNecessary (string markdownFilePath, IEnumerable <string>? translationLanguages)
        {
            try
            {
                if (translationLanguages == null)
                    return 0;

                string xTranslatedDirectoryPath = yyPath.Join (Path.GetDirectoryName (markdownFilePath) ?? throw new yyUnexpectedNullException ("Directory path is missing."), "Translated");

                foreach (string xTranslationLanguage in translationLanguages)
                {
                    string xTranslationFileName = $"{Path.GetFileNameWithoutExtension (markdownFilePath)}-{xTranslationLanguage}{Path.GetExtension (markdownFilePath)}",
                           xTranslationFilePath = yyPath.Join (xTranslatedDirectoryPath, xTranslationFileName);

                    if (File.Exists (xTranslationFilePath))
                        continue; // Skipped silently.

                    string xMarkdownString = File.ReadAllText (markdownFilePath, yyEncoding.Default),
                           xTranslation = _Translate (xMarkdownString, xTranslationLanguage);

                    Directory.CreateDirectory (xTranslatedDirectoryPath);
                    File.WriteAllText (xTranslationFilePath, xTranslation, yyEncoding.Default);
                    Console.WriteLine ($"Translated: {Path.GetFileName (xTranslationFilePath)}"); // Not silent.
                }

                return 0;
            }

            catch (Exception xException)
            {
                Console.WriteLine ($"Translation failed: {Path.GetFileName (markdownFilePath)}");
                Console.WriteLine (xException.ToString ());
                return 1;
            }
        }

        void _SaveTranscription (string directoryPath, IList <string> fileNames, IEnumerable <string>? inconclusivePageFileNames, IEnumerable <string>? translationLanguages)
        {
            List <string> xInputFileNames = [];
            List <(string FileNameWithoutExtension, string Transcription)> xOutput = [];
            int xErrorCount = 0;

            for (int temp = 0; temp < fileNames.Count; temp ++)
            {
                string xInputFileName = fileNames [temp];
                xInputFileNames.Add (xInputFileName);

                if (inconclusivePageFileNames != null &&
                    inconclusivePageFileNames.Contains (xInputFileName, StringComparer.OrdinalIgnoreCase) &&
                    temp < fileNames.Count - 1)
                        continue;

                string xOutputFileNameWithoutExtension = _GetSimplifiedFileNameWithoutExtension (xInputFileName),
                       xJsonFilePath = yyPath.Join (directoryPath, "Transcribed", xOutputFileNameWithoutExtension + ".json"),
                       xMarkdownFilePath = yyPath.Join (directoryPath, "Transcribed", xOutputFileNameWithoutExtension + ".md");

                if (File.Exists (xJsonFilePath) && File.Exists (xMarkdownFilePath))
                {
                    string xTranscription = File.ReadAllText (xMarkdownFilePath, yyEncoding.Default);
                    xOutput.Add ((xOutputFileNameWithoutExtension, xTranscription));
                    Console.WriteLine ($"Skipped: {Path.GetFileName (xMarkdownFilePath)}");
                    xErrorCount += _TranslateIfNecessary (xMarkdownFilePath, translationLanguages);
                    continue;
                }

                string? xPrompt = xOutput.Count == 0 ? "The first image is a front page." : null;

                try
                {
                    var xResult = _TranscribeImages (directoryPath, xInputFileNames, xPrompt);
                    Directory.CreateDirectory (yyPath.Join (directoryPath, "Transcribed"));
                    File.WriteAllText (xJsonFilePath, xResult.ResponseJsonString, yyEncoding.Default);
                    File.WriteAllText (xMarkdownFilePath, xResult.Transcription, yyEncoding.Default);
                    xOutput.Add ((xOutputFileNameWithoutExtension, xResult.Transcription));
                    Console.WriteLine ($"Saved: {Path.GetFileName (xMarkdownFilePath)}");
                    xErrorCount += _TranslateIfNecessary (xMarkdownFilePath, translationLanguages);
                }

                catch (Exception xException)
                {
                    xErrorCount ++;
                    Console.WriteLine ($"Transcription failed: {string.Join (", ", xInputFileNames)}");
                    Console.WriteLine (xException.ToString ());
                    continue;
                }

                finally
                {
                    xInputFileNames.Clear ();
                }
            }

            if (xErrorCount > 0 || xOutput.Count <= 1)
                return;

            string xMergedMarkdownFilePath = yyPath.Join (directoryPath, "Transcribed", xOutput.First ().FileNameWithoutExtension + "-Merged.md"),
            xMergedMarkdownString = string.Join (Environment.NewLine + Environment.NewLine, xOutput.Select (x => x.Transcription.Optimize ()));
            File.WriteAllText (xMergedMarkdownFilePath, xMergedMarkdownString, yyEncoding.Default);
            Console.WriteLine ($"Merged: {Path.GetFileName (xMergedMarkdownFilePath)}");
        }

        string xDirectoryPath = @"C:\Repositories\Shared\Scans\2025\エカの免許";

        void _Go (IList <string> fileNames, IEnumerable <string>? inconclusivePageFileNames = null) =>
            _SaveTranscription (xDirectoryPath, fileNames, inconclusivePageFileNames, [ "English", "Russian" ]);

        _Go ([ "20250312125048.jpg", "20250312125048_001.jpg", "20250312125048_002.jpg", "20250312125048_003.jpg" ]);
        _Go ([ "20250312125404.jpg", "20250312125404_001.jpg" ]);
        _Go ([ "20250312125404_002.jpg" ]);
        _Go ([ "20250312125404_003.jpg" ]);
        _Go ([ "20250312125404_004.jpg" ]);
        _Go ([ "20250312125528.jpg" ]);

        xDirectoryPath = yyPath.Join (xDirectoryPath, "技能試験受験のしおり");

        _Go (
        [
            "20250312124221.jpg",
            "20250312124221_001.jpg",
            "20250312124221_002.jpg",
            "20250312124221_003.jpg",
            "20250312124221_004.jpg",
            "20250312124221_005.jpg",
            "20250312124221_006.jpg",
            "20250312124221_007.jpg",
            "20250312124221_008.jpg",
            "20250312124221_009.jpg",
            "20250312124221_010.jpg",
            "20250312124221_011.jpg",
            "20250312124221_012.jpg",
            "20250312124221_013.jpg",
            "20250312124221_014.jpg",
            "20250312124221_015.jpg",
            "20250312124221_016.jpg",
            "20250312124221_017.jpg",
            "20250312124221_018.jpg",
            "20250312124221_019.jpg",
            "20250312124221_020.jpg",
            "20250312124221_021.jpg",
            "20250312124221_022.jpg",
            "20250312124221_023.jpg",
            "20250312124221_024.jpg",
            "20250312124221_025.jpg",
            "20250312124221_026.jpg",
            "20250312124221_027.jpg",
            "20250312124221_028.jpg",
            "20250312124221_029.jpg",
            "20250312124221_030.jpg",
            "20250312124221_031.jpg",
            "20250312124221_032.jpg",
            "20250312124221_033.jpg",
            "20250312124221_034.jpg",
            "20250312124221_035.jpg"
        ],
        [
            "20250312124221_002.jpg",
            "20250312124221_011.jpg",
        ]);
    }
}
