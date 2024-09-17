using System.Text.Json.Serialization;

namespace yyMailLib
{
    public class yyMailMessageAttachment
    {
        // To attach a file, we set OriginalFilePath and, optionally, NewFileName.
        // Before the message is sent, the attachment file is copied and its new path, that is relative to the location of the app, is stored in the instance.
        // We have the absolute path of the new base directory, so the current absolute path doesnt need to be in the instance.

        // http://www.mimekit.net/docs/html/T_MimeKit_MimeEntity.htm
        // http://www.mimekit.net/docs/html/T_MimeKit_ContentDisposition.htm

        [JsonPropertyName ("original_file_path")]
        public string? OriginalFilePath { get; set; }

        [JsonPropertyName ("new_file_name")]
        public string? NewFileName { get; set; }

        [JsonPropertyName ("current_relative_file_path")]
        public string? CurrentRelativeFilePath { get; set; }

        [JsonPropertyName ("created_at_utc")]
        public DateTime? CreatedAtUtc { get; set; }

        [JsonPropertyName ("modification_utc")]
        public DateTime? ModificationUtc { get; set; }

        [JsonPropertyName ("read_utc")]
        public DateTime? ReadUtc { get; set; }

        [JsonPropertyName ("content_length")]
        public long? ContentLength { get; set; }

        public yyMailMessageAttachment ()
        {
        }

        public yyMailMessageAttachment (string originalFilePath, string? newFileName = null)
        {
            OriginalFilePath = originalFilePath;
            NewFileName = newFileName ?? Path.GetFileName (originalFilePath);

            FileInfo xFile = new (originalFilePath);

            // ChatGPT says these timestamp-related properties are reliably available on Windows and Mac while creation time may not be on Linux due to file system limitations.
            // The source code suggests that there are fallback mechanisms in place for Linux:
            // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/FileStatus.Unix.cs,2b4046d793b2bb3a

            CreatedAtUtc = xFile.CreationTimeUtc;
            ModificationUtc = xFile.LastWriteTimeUtc;
            ReadUtc = xFile.LastAccessTimeUtc;
            ContentLength = xFile.Length;
        }
    }
}
