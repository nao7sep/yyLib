﻿using System.Diagnostics.CodeAnalysis;
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
        public IList <yyLog> RecentLogs { get; } = [];

        // Other methods for RecentLogs removed.
        // Currently, it's my laziness-based design pattern that only Add* methods are defined for collections within classes.
        // This makes some sense because creating and adding an object can often be sugar-coated effectively,
        // while there are few ways to shorten code that accesses nested properties and methods.

        public void AddRecentLog (yyLog log) => RecentLogs.Add (log);

        [JsonPropertyName ("writes_to_text_file")]
        [ConfigurationKeyName ("writes_to_text_file")]
        public bool? WritesToTextFile { get; set; }

        [JsonPropertyName ("text_log_writer")]
        [ConfigurationKeyName ("text_log_writer")]
        public yyTextLogWriter? TextLogWriter { get; set; }

        [JsonPropertyName ("writes_to_json_files")]
        [ConfigurationKeyName ("writes_to_json_files")]
        public bool? WritesToJsonFiles { get; set; }

        [JsonPropertyName ("json_log_writer")]
        [ConfigurationKeyName ("json_log_writer")]
        public yyJsonLogWriter? JsonLogWriter { get; set; }

        [JsonPropertyName ("writes_to_sqlite_database")]
        [ConfigurationKeyName ("writes_to_sqlite_database")]
        public bool? WritesToSqliteDatabase { get; set; }

        [JsonPropertyName ("sqlite_log_writer")]
        [ConfigurationKeyName ("sqlite_log_writer")]
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

        // Suppresses the warning about catching general exceptions (CA1031).
        [SuppressMessage ("Design", "CA1031")]
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

        // ⚠️ Avoid 'const' for values that may change in the future!
        //
        // 🔹 Why?
        // - 'const' values are **inlined at compile time**, meaning updates require recompilation.
        // - This can lead to outdated values in consuming assemblies, causing unexpected bugs.
        //
        // 🔹 Use 'const' for truly immutable values (e.g., Pi).
        // 🔹 Use 'static readonly' for config values, file paths, or settings to allow updates at runtime.

        // Disables the warning for explicitly initializing a default value (CA1805).
        [SuppressMessage ("Performance", "CA1805")]
        public static readonly bool DefaultWritesToTextFile = false;
        public static readonly string TextLogWriterDefaultRelativeFilePath = "Logs.txt";
        public static readonly string TextLogWriterDefaultEncodingName = "utf-8";

        public static readonly bool DefaultWritesToJsonFiles = true;
        public static readonly string JsonLogWriterDefaultRelativeDirectoryPath = "Logs";
        public static readonly string JsonLogWriterDefaultEncodingName = "utf-8";

        // Disables the warning for explicitly initializing a default value (CA1805).
        [SuppressMessage ("Performance", "CA1805")]
        public static readonly bool DefaultWritesToSqliteDatabase = false;
        public static readonly string SqliteLogWriterDefaultRelativeFilePath = "Logs.db";
        public static readonly string SqliteLogWriterDefaultTableName = "Logs";

        private static yyLogger _CreateDefault ()
        {
            var xLoggerSection = yyAppSettings.Config.GetSection ("logger");

            if (xLoggerSection.Exists () &&
                xLoggerSection.GetChildren ().Any () &&
                xLoggerSection.Get <yyLogger> () is { } xLogger)
                    return xLogger;

            if (yyUserSecrets.Default.Logger != null)
                return yyUserSecrets.Default.Logger;

            return new yyLogger
            {
                WritesToTextFile = DefaultWritesToTextFile,

                TextLogWriter = new yyTextLogWriter
                {
                    RelativeFilePath = TextLogWriterDefaultRelativeFilePath,
                    EncodingName = TextLogWriterDefaultEncodingName
                },

                WritesToJsonFiles = DefaultWritesToJsonFiles,

                JsonLogWriter = new yyJsonLogWriter
                {
                    RelativeDirectoryPath = JsonLogWriterDefaultRelativeDirectoryPath,
                    EncodingName = JsonLogWriterDefaultEncodingName
                },

                WritesToSqliteDatabase = DefaultWritesToSqliteDatabase,

                SqliteLogWriter = new yySqliteLogWriter
                {
                    RelativeFilePath = SqliteLogWriterDefaultRelativeFilePath,
                    TableName = SqliteLogWriterDefaultTableName
                }
            };
        }

        private static readonly Lazy <yyLogger> _default = new (_CreateDefault ());

        public static yyLogger Default => _default.Value;
    }
}
