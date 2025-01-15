using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    // Writes logs as key-value pairs to a text file and/or a number of JSON files. => And also to a SQLite database.
    // 'RecentLogs' contains only the logs that have been written during the current session.

    // By default, logs are written only to JSON files as the JSON mode is almost thread-safe (but not completely). => Added a lock to the text mode.
    // We can change WritesToTextFile/WritesToJsonFiles anytime as there's no caching involved. => And also WritesToSqliteDatabase.
    // Added comment: JSON is still the default mode as it's reliable, entry-based and human-readable.

    // The yyLogger class previously inherited from IEnumerable<yyLog> to allow iteration over logs.
    // However, this caused issues with JSON deserialization because System.Text.Json mistakenly
    // treated yyLogger as a collection rather than an object. As a result, any JSON nodes that
    // mapped to yyLogger were incorrectly expected to be arrays, leading to deserialization failures.

    public class yyLogger // : IEnumerable <yyLog>
    {
        /// <summary>
        /// Does not reload logs from previous sessions.
        /// Consider this to be a toilet-type, last-chance log storage.
        /// </summary>
        [JsonIgnore]
        public List <yyLog> RecentLogs { get; } = [];

        // Other methods for RecentLogs removed.
        // Currently, it's my laziness-based design pattern that only Add* methods are defined for collections within classes.
        // This makes some sense because creating and adding an object can often be sugar-coated effectively,
        // while there are few ways to shorten code that accesses nested properties and methods.

        public void AddRecentLog (yyLog log) => RecentLogs.Add (log);

        [JsonPropertyName ("writes_to_text_file")]
        public bool? WritesToTextFile { get; set; }

        [JsonPropertyName ("text_log_writer")]
        public yyTextLogWriter? TextLogWriter { get; set; }

        [JsonPropertyName ("writes_to_json_files")]
        public bool? WritesToJsonFiles { get; set; }

        [JsonPropertyName ("json_log_writer")]
        public yyJsonLogWriter? JsonLogWriter { get; set; }

        [JsonPropertyName ("writes_to_sqlite_database")]
        public bool? WritesToSqliteDatabase { get; set; }

        [JsonPropertyName ("sqlite_log_writer")]
        public yySqliteLogWriter? SqliteLogWriter { get; set; }

        /// <summary>
        /// Use TryWrite unless you need to handle exceptions.
        /// </summary>
        public void Write (string key, string value)
        {
            DateTime xCreatedAtUtc = DateTime.UtcNow;

            AddRecentLog (new ()
            {
                CreatedAtUtc = xCreatedAtUtc,
                Key = key,
                Value = value
            });

            // Doesnt throw an exception when the log is not written to anywhere for 2 reasons:
            // 1) We might eventually need to add a writer that writes to a database or the OS event log. => Just added one.
            // 2) Loggers shouldnt throw exceptions and the recommended TryWrite methods will ignore any exceptions.

            if (WritesToTextFile == true)
                TextLogWriter?.Write (xCreatedAtUtc, key, value);

            if (WritesToJsonFiles == true)
                JsonLogWriter?.Write (xCreatedAtUtc, key, value);

            if (WritesToSqliteDatabase == true)
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
        // Default
        // -----------------------------------------------------------------------------

        private static yyLogger _CreateDefault ()
        {
            if (yyAppSettings.Config.GetSection ("logger").Get <yyLogger> () is { } xLogger)
                return xLogger;

            if (yyUserSecrets.Default.Logger != null)
                return yyUserSecrets.Default.Logger;

            return new yyLogger ();
        }

        private static readonly Lazy <yyLogger> _default = new (_CreateDefault ());

        public static yyLogger Default => _default.Value;
    }
}
