using System.Text;
using System.Text.Json;

namespace yyLib
{
    public class yySimpleLogJsonWriter
    {
        public string DirectoryPath { get; private set; }

        public Encoding Encoding { get; private set; }

        public yySimpleLogJsonWriter (string directoryPath, Encoding? encoding = null)
        {
            DirectoryPath = directoryPath;
            Encoding = encoding ?? Encoding.UTF8;
        }

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
