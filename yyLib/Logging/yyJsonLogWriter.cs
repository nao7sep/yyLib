using System.Text;
using System.Text.Json;

namespace yyLib
{
    public class yyJsonLogWriter (string directoryPath, Encoding? encoding = null)
    {
        public string DirectoryPath { get; init; } = directoryPath;

        public Encoding Encoding { get; init; } = encoding ?? Encoding.UTF8;

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            string xFileName = $"Log-{createdAtUtc.ToRoundtripFileNameString ()}.json",
                xFilePath = Path.Join (DirectoryPath, xFileName); // Should be unique enough.

            yyLogEntry xEntry = new ()
            {
                CreatedAtUtc = createdAtUtc,
                Key = key,
                Value = value
            };

            string xFileContents = JsonSerializer.Serialize (xEntry, yyJson.DefaultSerializationOptions);

            yyDirectory.Create (DirectoryPath);
            File.WriteAllText (xFilePath, xFileContents, Encoding);
        }
    }
}
