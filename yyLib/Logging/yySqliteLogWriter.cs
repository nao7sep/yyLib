using System.Data.SQLite;

namespace yyLib
{
    public class yySqliteLogWriter (string connectionString, string tableName): yyLogWriter
    {
        private static readonly object _lock = new ();

        public string ConnectionString { get; init; } = connectionString;

        public string TableName { get; init; } = tableName;

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
                        xCommand.Parameters.AddWithValue ("@CreatedAtUtc", yyConvertor.DateTimeToRoundtripString (createdAtUtc));
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
