using System.Text;
using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyTextLogWriter: yyLogWriterInterface
    {
        private static readonly object _lock = new ();

        private string? _relativeFilePath;

        [JsonPropertyName ("relative_file_path")]
        public string? RelativeFilePath
        {
            get => _relativeFilePath;

            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace (value) || Path.IsPathFullyQualified (value))
                        throw new yyInvalidDataException ($"'{nameof (RelativeFilePath)}' is invalid: {value.GetVisibleString ()}");

                    _relativeFilePath = value;
                    FilePath = yyAppDirectory.MapPath (value);
                }

                // This is a nullable property, which can actually be set to null.
                else
                {
                    _relativeFilePath = null;
                    FilePath = null;
                }
            }
        }

        [JsonIgnore]
        public string? FilePath { get; private set; }

        private string? _encodingName;

        [JsonPropertyName ("encoding_name")]
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

        [JsonIgnore]
        public Encoding? Encoding { get; private set; }

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            lock (_lock)
            {
                StringBuilder xBuilder = new ();

                if (File.Exists (FilePath))
                    xBuilder.AppendLine ("----");

                xBuilder.AppendLine ($"UTC: {yyConverter.DateTimeToRoundtripString (createdAtUtc)}");
                xBuilder.AppendLine ($"{key}: {value.TrimRedundantLines ()}"); // Auto trimmed.

                yyDirectory.CreateParent (FilePath!); // Should throw if null.
                File.AppendAllText (FilePath!, xBuilder.ToString (), Encoding ?? Encoding.UTF8);
            }
        }
    }
}
