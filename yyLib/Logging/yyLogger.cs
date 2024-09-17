using System.Collections;
using System.Text;

namespace yyLib
{
    // Writes log entries as key-value pairs to a text file and/or a number of JSON files.
    // 'RecentEntries' contains only the entries that have been written during the current session.

    // By default, log entries are written only to JSON files as the JSON mode is almost thread-safe (but not completely).
    // We can change WritesToTextFile/WritesToJsonFiles anytime as there's no caching involved.

    public class yyLogger: IEnumerable <yyLogEntry>
    {
        public List <yyLogEntry> RecentEntries { get; } = [];

        public int RecentEntryCount => RecentEntries.Count;

        public yyLogEntry this [int index]
        {
            get => RecentEntries [index];
            set => RecentEntries [index] = value;
        }

        public bool Contains (yyLogEntry log) => RecentEntries.Contains (log);

        public void Add (yyLogEntry log) => RecentEntries.Add (log);

        public IEnumerator <yyLogEntry> GetEnumerator () => RecentEntries.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

        public void CopyTo (yyLogEntry [] array, int arrayIndex) => RecentEntries.CopyTo (array, arrayIndex);

        public bool Remove (yyLogEntry log) => RecentEntries.Remove (log);

        public void Clear () => RecentEntries.Clear ();

        public bool WritesToTextFile { get; set; }

        public yyTextLogWriter? TextLogWriter { get; init; }

        public bool WritesToJsonFiles { get; set; }

        public yyJsonLogWriter? JsonLogWriter { get; init; }

        public yyLogger (bool writesToTextFile = false, string? textLogWriterFilePath = null, bool writesToJsonFiles = true, string? jsonLogWriterDirectoryPath = null, Encoding? encoding = null)
        {
            // The boolean values indicating whether to write in each mode and the actual paths to write to are decoupled.
            // The former must have the right values only upon writing each log entry.
            // The latter are init properties and therefore must be initialized in the constructor.

            if (textLogWriterFilePath == null && jsonLogWriterDirectoryPath == null)
                throw new yyArgumentException ($"Either '{nameof (textLogWriterFilePath)}' or '{nameof (jsonLogWriterDirectoryPath)}' must be specified.");

            WritesToTextFile = writesToTextFile;

            if (textLogWriterFilePath != null)
                TextLogWriter = new (textLogWriterFilePath, encoding ?? Encoding.UTF8);

            WritesToJsonFiles = writesToJsonFiles;

            if (jsonLogWriterDirectoryPath != null)
                JsonLogWriter = new (jsonLogWriterDirectoryPath, encoding ?? Encoding.UTF8);
        }

        /// <summary>
        /// Use TryWrite unless you need to handle exceptions.
        /// </summary>
        public void Write (string key, string value)
        {
            DateTime xCreatedAtUtc = DateTime.UtcNow;

            RecentEntries.Add (new ()
            {
                CreatedAtUtc = xCreatedAtUtc,
                Key = key,
                Value = value
            });

            // Doesnt throw an exception when the log entry is not written to anywhere for 2 reasons:
            // 1) We might eventually need to add a writer that writes to a database or the OS event log.
            // 2) Loggers shouldnt throw exceptions and the recommended TryWrite methods will ignore any exceptions.

            if (WritesToTextFile)
                TextLogWriter?.Write (xCreatedAtUtc, key, value);

            if (WritesToJsonFiles)
                JsonLogWriter?.Write (xCreatedAtUtc, key, value);
        }

        /// <summary>
        /// Use TryWriteMessage unless you need to handle exceptions.
        /// </summary>
        public void WriteMessage (string message) => Write ("Message", message);

        /// <summary>
        /// Use TryWriteException unless you need to handle exceptions.
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

        // -----------------------------------------------------------------------------
        //     Static Members
        // -----------------------------------------------------------------------------

        private static readonly Lazy <string> _defaultTextLogWriterFilePath = new (() => yyAppDirectory.MapPath ("Logs.txt"));

        public static string DefaultTextLogWriterFilePath => _defaultTextLogWriterFilePath.Value;

        private static readonly Lazy <string> _defaultJsonLogWriterDirectoryPath = new (() => yyAppDirectory.MapPath ("Logs"));

        public static string DefaultJsonLogWriterDirectoryPath => _defaultJsonLogWriterDirectoryPath.Value;

        private static readonly Lazy <yyLogger> _default = new (() =>
            new yyLogger (writesToTextFile: false, DefaultTextLogWriterFilePath,
                          writesToJsonFiles: true, DefaultJsonLogWriterDirectoryPath));

        /// <summary>
        /// NOT thread-safe.
        /// </summary>
        public static yyLogger Default => _default.Value;

    }
}
