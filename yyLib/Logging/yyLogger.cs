using System.Collections;
using System.Text;

namespace yyLib
{
    // Writes logs as key-value pairs to a text file and/or a number of JSON files. => And also to a SQLite database.
    // 'RecentLogs' contains only the logs that have been written during the current session.

    // By default, logs are written only to JSON files as the JSON mode is almost thread-safe (but not completely).
    // We can change WritesToTextFile/WritesToJsonFiles anytime as there's no caching involved. => And also WritesToSqliteDatabase.
    // Added comment: JSON is still the default mode as it's reliable and human-readable.

    public class yyLogger: IEnumerable <yyLog>
    {
        public List <yyLog> RecentLogs { get; } = [];

        public int RecentLogCount => RecentLogs.Count;

        public yyLog this [int index]
        {
            get => RecentLogs [index];
            set => RecentLogs [index] = value;
        }

        public bool Contains (yyLog log) => RecentLogs.Contains (log);

        public void Add (yyLog log) => RecentLogs.Add (log);

        public IEnumerator <yyLog> GetEnumerator () => RecentLogs.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

        public void CopyTo (yyLog [] array, int arrayIndex) => RecentLogs.CopyTo (array, arrayIndex);

        public bool Remove (yyLog log) => RecentLogs.Remove (log);

        public void Clear () => RecentLogs.Clear ();

        // -----------------------------------------------------------------------------

        public bool WritesToTextFile { get; set; }

        public yyTextLogWriter? TextLogWriter { get; init; }

        public bool WritesToJsonFiles { get; set; }

        public yyJsonLogWriter? JsonLogWriter { get; init; }

        public bool WritesToSqliteDatabase { get; set; }

        public yySqliteLogWriter? SqliteLogWriter { get; init; }

        public yyLogger (bool writesToTextFile = false, string? textLogWriterFilePath = null,
                         bool writesToJsonFiles = false, string? jsonLogWriterDirectoryPath = null, // writesToJsonFiles used to default to true.
                         bool writesToSqliteDatabase = false, string? sqliteLogWriterConnectionString = null, string? sqliteLogWriterTableName = null,
                         Encoding? encoding = null)
        {
            // The boolean values indicating whether to write in each mode and the actual paths to write to are decoupled.
            // The former must have the right values only upon writing each log.
            // The latter are init properties and therefore must be initialized in the constructor.

            if (textLogWriterFilePath == null && jsonLogWriterDirectoryPath == null && (sqliteLogWriterConnectionString == null || sqliteLogWriterTableName == null))
                throw new yyArgumentException ($"Either '{nameof (textLogWriterFilePath)}' or '{nameof (jsonLogWriterDirectoryPath)}' or '{nameof (sqliteLogWriterConnectionString)}' and '{nameof (sqliteLogWriterTableName)}' must be specified.");

            WritesToTextFile = writesToTextFile;

            if (textLogWriterFilePath != null)
                TextLogWriter = new (textLogWriterFilePath, encoding ?? Encoding.UTF8);

            WritesToJsonFiles = writesToJsonFiles;

            if (jsonLogWriterDirectoryPath != null)
                JsonLogWriter = new (jsonLogWriterDirectoryPath, encoding ?? Encoding.UTF8);

            WritesToSqliteDatabase = writesToSqliteDatabase;

            if (sqliteLogWriterConnectionString != null && sqliteLogWriterTableName != null)
                SqliteLogWriter = new (sqliteLogWriterConnectionString, sqliteLogWriterTableName);
        }

        /// <summary>
        /// Use TryWrite unless you need to handle exceptions.
        /// </summary>
        public void Write (string key, string value)
        {
            DateTime xCreatedAtUtc = DateTime.UtcNow;

            RecentLogs.Add (new ()
            {
                CreatedAtUtc = xCreatedAtUtc,
                Key = key,
                Value = value
            });

            // Doesnt throw an exception when the log is not written to anywhere for 2 reasons:
            // 1) We might eventually need to add a writer that writes to a database or the OS event log. => Just added one.
            // 2) Loggers shouldnt throw exceptions and the recommended TryWrite methods will ignore any exceptions.

            if (WritesToTextFile)
                TextLogWriter?.Write (xCreatedAtUtc, key, value);

            if (WritesToJsonFiles)
                JsonLogWriter?.Write (xCreatedAtUtc, key, value);

            if (WritesToSqliteDatabase)
                SqliteLogWriter?.Write (xCreatedAtUtc, key, value);
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

        private static readonly Lazy <string> _defaultSqliteLogWriterConnectionString = new (() => $"Data Source={yyAppDirectory.MapPath ("Logs.db")}");

        public static string DefaultSqliteLogWriterConnectionString => _defaultSqliteLogWriterConnectionString.Value;

        public static string DefaultSqliteLogWriterTableName { get; } = "Logs";

        private static readonly Lazy <yyLogger> _default = new (() =>
            new yyLogger (writesToTextFile: false, DefaultTextLogWriterFilePath,
                          writesToJsonFiles: true, DefaultJsonLogWriterDirectoryPath,
                          writesToSqliteDatabase: false, DefaultSqliteLogWriterConnectionString, DefaultSqliteLogWriterTableName));

        /// <summary>
        /// NOT thread-safe.
        /// </summary>
        public static yyLogger Default => _default.Value;
    }
}
