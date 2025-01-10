using System.Text;
using System.Text.Json;

namespace yyLib
{
    public class yyJsonLogWriter (string directoryPath, Encoding? encoding = null): yyLogWriterInterface
    {
        public string DirectoryPath { get; init; } = directoryPath;

        public Encoding Encoding { get; init; } = encoding ?? Encoding.UTF8;

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            // There's no point in locking here because the file name will be based on createdAtUtc.
            // We can only hope that createdAtUtc will be unique enough to prevent conflicts.

            string xFileName = $"Log-{yyConvertor.DateTimeToRoundtripFileNameString (createdAtUtc)}.json",
                xFilePath = Path.Join (DirectoryPath, xFileName);

            yyLog xLog = new ()
            {
                CreatedAtUtc = createdAtUtc,
                Key = key,
                Value = value
            };

            string xFileContents = JsonSerializer.Serialize (xLog, yyJson.DefaultSerializationOptions);

            yyDirectory.Create (DirectoryPath);
            File.WriteAllText (xFilePath, xFileContents, Encoding);
        }
    }
}
