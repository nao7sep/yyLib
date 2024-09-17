using System.Text;
using System.Text.Json;

namespace yyLib
{
    public class yySimpleLogJsonWriter (string directoryPath, Encoding? encoding = null)
    {
        public string DirectoryPath { get; private set; } = directoryPath;

        public Encoding Encoding { get; private set; } = encoding ?? Encoding.UTF8;

        public void Write (DateTime creationUtc, string key, string value)
        {
            string xFileName = $"Log-{creationUtc.ToRoundtripFileNameString ()}.json",
                xFilePath = Path.Join (DirectoryPath, xFileName); // Should be unique enough.

            yySimpleLog xLog = new ()
            {
                CreationUtc = creationUtc,
                Key = key,
                Value = value
            };

            string xFileContents = JsonSerializer.Serialize (xLog, yyJson.DefaultSerializationOptions);

            yyDirectory.Create (DirectoryPath);
            File.WriteAllText (xFilePath, xFileContents, Encoding);
        }
    }
}
