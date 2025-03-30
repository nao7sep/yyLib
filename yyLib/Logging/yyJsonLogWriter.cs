using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyJsonLogWriter: yyLogWriterInterface
    {
        private string? _relativeDirectoryPath;

        [JsonPropertyName ("relative_directory_path")]
        [ConfigurationKeyName ("relative_directory_path")]
        public string? RelativeDirectoryPath
        {
            get => _relativeDirectoryPath;

            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace (value) || Path.IsPathFullyQualified (value))
                        throw new yyInvalidDataException ($"'{nameof (RelativeDirectoryPath)}' is invalid: {value.GetVisibleString ()}");

                    _relativeDirectoryPath = value;
                    DirectoryPath = yyAppDirectory.MapPath (value);
                }

                // This is a nullable property, which can actually be set to null.
                else
                {
                    _relativeDirectoryPath = null;
                    DirectoryPath = null;
                }
            }
        }

        [JsonIgnore]
        public string? DirectoryPath { get; private set; }

        private string? _encodingName;

        [JsonPropertyName ("encoding_name")]
        [ConfigurationKeyName ("encoding_name")]
        public string? EncodingName
        {
            get => _encodingName;

            set
            {
                if (value != null)
                {
                    // GetEncoding must be called first to throw an exception if the encoding name is invalid.
                    Encoding = Encoding.GetEncoding (value);
                    _encodingName = value;
                }

                else
                {
                    _encodingName = null;
                    Encoding = null;
                }
            }
        }

        // I havent found a way to specify a custom converter to IConfiguration's default binder.

        [JsonIgnore]
        public Encoding? Encoding { get; private set; }

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            // There's no point in locking here because the file name will be based on createdAtUtc.
            // We can only hope that createdAtUtc will be unique enough to prevent conflicts.

            if (DirectoryPath == null)
                throw new yyInvalidOperationException ($"'{nameof (DirectoryPath)}' is not set.");

            string xFileName = $"Log-{yyConverter.DateTimeToRoundtripFileNameString (createdAtUtc)}.json",
                xFilePath = yyPath.Join (DirectoryPath, xFileName);

            yyLog xLog = new ()
            {
                CreatedAtUtc = createdAtUtc,
                Key = key,
                Value = value
            };

            string xFileContents = JsonSerializer.Serialize (xLog, yyJson.DefaultSerializationOptions);

            yyDirectory.Create (DirectoryPath);
            File.WriteAllText (xFilePath, xFileContents, Encoding.OrDefaultEncoding ());
        }
    }
}
