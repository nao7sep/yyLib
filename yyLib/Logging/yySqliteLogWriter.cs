using System.Data.SQLite;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yySqliteLogWriter: yyLogWriterInterface
    {
        private static readonly Lock _lock = new ();

        private string? _relativeFilePath;

        [JsonPropertyName ("relative_file_path")]
        [ConfigurationKeyName ("relative_file_path")]
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
                    ConnectionString = $"Data Source={yyAppDirectory.MapPath (value)}";
                }

                // This is a nullable property, which can actually be set to null.
                else
                {
                    _relativeFilePath = null;
                    ConnectionString = null;
                }
            }
        }

        [JsonIgnore]
        public string? ConnectionString { get; private set; }

        [JsonPropertyName ("table_name")]
        [ConfigurationKeyName ("table_name")]
        public string? TableName { get; set; }

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            lock (_lock)
            {
                using SQLiteConnection xConnection = new (ConnectionString);
                xConnection.Open ();

                using SQLiteTransaction xTransaction = xConnection.BeginTransaction ();

                try
                {
                    using (SQLiteCommand xCommand = new (
                        $"CREATE TABLE IF NOT EXISTS {TableName} (" +
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "CreatedAtUtc TEXT NOT NULL, " +
                        "Key TEXT NOT NULL, " +
                        "Value TEXT NOT NULL)", xConnection))
                    {
                        xCommand.ExecuteNonQuery ();
                    }

                    using (SQLiteCommand xCommand = new (
                        $"INSERT INTO {TableName} (CreatedAtUtc, Key, Value) " +
                        "VALUES (@CreatedAtUtc, @Key, @Value)", xConnection))
                    {
                        xCommand.Parameters.AddWithValue ("@CreatedAtUtc", yyConverter.DateTimeToRoundtripString (createdAtUtc));
                        xCommand.Parameters.AddWithValue ("@Key", key);
                        xCommand.Parameters.AddWithValue ("@Value", value);
                        xCommand.ExecuteNonQuery ();
                    }

                    xTransaction.Commit ();
                }

                catch
                {
                    xTransaction.Rollback ();
                    throw;
                }
            }
        }
    }
}
