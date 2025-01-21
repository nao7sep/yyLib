using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyMailAttachment
    {
        // To attach a file, we set OriginalFilePath and, optionally, NewFileName.
        // Before the message is sent, the attachment file is copied and its new path, that is relative to the location of the app, is stored in the instance.
        // We have the absolute path of the new base directory, so the current absolute path doesnt need to be in the instance.

        // http://www.mimekit.net/docs/html/T_MimeKit_MimeEntity.htm
        // http://www.mimekit.net/docs/html/T_MimeKit_ContentDisposition.htm

        // Nullable just to silence the compiler.
        private string? _originalFilePath;

        [JsonPropertyName ("original_file_path")]
        public required string OriginalFilePath
        {
            get => _originalFilePath!;

            set
            {
                // If the file has been sent and saved somewhere else,
                // the file may not be available at the original path, which is OK.
                // We just need to make sure the path is fully qualified.

                if (Path.IsPathFullyQualified (value) == false)
                    throw new yyArgumentException ($"The path '{value}' is not fully qualified.");

                _originalFilePath = value;

                FileInfo xFile = new (value);

                if (xFile.Exists)
                {
                    // ChatGPT says these timestamp-related properties are reliably available on Windows and Mac,
                    // while creation time may not be on Linux due to file system limitations.
                    // The source code suggests that there are fallback mechanisms in place for Linux:
                    // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/FileStatus.Unix.cs,2b4046d793b2bb3a

                    CreatedAtUtc = xFile.CreationTimeUtc;
                    ModifiedAtUtc = xFile.LastWriteTimeUtc;
                    ReadAtUtc = xFile.LastAccessTimeUtc;
                    ContentLength = xFile.Length;
                }
            }
        }

        private string? _newFileName;

        /// <summary>
        /// NOT auto-generated from OriginalFilePath.
        /// Use the ?? operator when referencing this property.
        /// </summary>
        [JsonPropertyName ("new_file_name")]
        public string? NewFileName
        {
            get => _newFileName;

            set
            {
                if (string.IsNullOrWhiteSpace (value))
                    throw new yyArgumentException ("The new file name is null or empty.");

                if (Path.IsPathFullyQualified (value))
                    throw new yyArgumentException ($"The path '{value}' is fully qualified.");

                _newFileName = value;
            }
        }

        private string? _currentRelativeFilePath;

        [JsonPropertyName ("current_relative_file_path")]
        public string? CurrentRelativeFilePath
        {
            get => _currentRelativeFilePath;

            set
            {
                if (string.IsNullOrWhiteSpace (value))
                    throw new yyArgumentException ("The current relative file path is null or empty.");

                if (Path.IsPathFullyQualified (value))
                    throw new yyArgumentException ($"The path '{value}' is fully qualified.");

                _currentRelativeFilePath = value;
            }
        }

        /// <summary>
        /// Auto-generated from OriginalFilePath.
        /// Settable because the metadata must be restorable from other sources.
        /// </summary>
        [JsonPropertyName ("created_at_utc")]
        public DateTime? CreatedAtUtc { get; set; }

        /// <summary>
        /// Auto-generated from OriginalFilePath.
        /// Settable because the metadata must be restorable from other sources.
        /// </summary>
        [JsonPropertyName ("modified_at_utc")]
        public DateTime? ModifiedAtUtc { get; set; }

        /// <summary>
        /// Auto-generated from OriginalFilePath.
        /// Settable because the metadata must be restorable from other sources.
        /// </summary>
        [JsonPropertyName ("read_at_utc")]
        public DateTime? ReadAtUtc { get; set; }

        /// <summary>
        /// Auto-generated from OriginalFilePath.
        /// Settable because the metadata must be restorable from other sources.
        /// </summary>
        [JsonPropertyName ("content_length")]
        public long? ContentLength { get; set; }
    }
}
