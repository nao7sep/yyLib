using System.Text;

namespace yyLib
{
    public class yySimpleLogger
    {
        // todo: Default properties for the paths.
        // todo: Comment on DefaultEncoding; it's considered more like Environment.NewLine.

        private static readonly Lazy <yySimpleLogger> _default = new (() =>
            new yySimpleLogger (yyApplicationDirectory.MapPath ("Logs.txt"), yyApplicationDirectory.MapPath ("Logs")));

        public static yySimpleLogger Default => _default.Value;

        public yySimpleLogTextWriter? TextWriter { get; private set; }

        public yySimpleLogJsonWriter? JsonWriter { get; private set; }

        public yySimpleLogger (string? textWriterFilePath = null, string? jsonWriterDirectoryPath = null, Encoding? encoding = null)
        {
            // Only null is accepted as a valid value to specify that the value shouldnt be used.
            // If either is an empty string, an exception should be thrown somewhere.

            if (textWriterFilePath == null && jsonWriterDirectoryPath == null)
                throw new yyArgumentException (yyMessage.Create ($"Either '{nameof (textWriterFilePath)}' or '{nameof (jsonWriterDirectoryPath)}' must be specified."));

            if (textWriterFilePath != null)
                TextWriter = new (textWriterFilePath, encoding ?? Encoding.UTF8);

            if (jsonWriterDirectoryPath != null)
                JsonWriter = new (jsonWriterDirectoryPath, encoding ?? Encoding.UTF8);
        }

        /// <summary>
        /// Use TryWrite instead.
        /// </summary>
        public void Write (string key, string value)
        {
            DateTime xCreationUtc = DateTime.UtcNow;

            TextWriter?.Write (xCreationUtc, key, value);
            JsonWriter?.Write (xCreationUtc, key, value);
        }

        /// <summary>
        /// Use TryWriteMessage instead.
        /// </summary>
        public void WriteMessage (string message) => Write ("Message", message);

        /// <summary>
        /// Use TryWriteException instead.
        /// </summary>
        public void WriteException (Exception exception) => Write ("Exception", exception.ToString ());

        // Safer methods.
        // If a logging method throws an exception in a catch block, the entire system might go down.

        public bool TryWrite (string key, string value)
        {
            try
            {
                Write (key, value);
                return true;
            }

            catch
            {
                return false;
            }
        }

        public bool TryWriteMessage (string message) => TryWrite ("Message", message);

        public bool TryWriteException (Exception exception) => TryWrite ("Exception", exception.ToString ());
    }
}
