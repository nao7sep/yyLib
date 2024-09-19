using System.Data.SQLite;

namespace yyLib
{
    public class yySqliteLogWriter (string connectionString, string tableName): yyLogWriter
    {
        public string ConnectionString { get; init; } = connectionString;

        public string TableName { get; init; } = tableName;

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            using SQLiteConnection xConnection = new (ConnectionString);

            xConnection.Open ();

            using (SQLiteCommand xCommand = new ($"CREATE TABLE IF NOT EXISTS {TableName} (CreatedAtUtc TEXT, Key TEXT, Value TEXT)", xConnection))
            {
                xCommand.ExecuteNonQuery ();
            }

            using (SQLiteCommand xCommand = new ($"INSERT INTO {TableName} (CreatedAtUtc, Key, Value) VALUES (@CreatedAtUtc, @Key, @Value)", xConnection))
            {
                xCommand.Parameters.AddWithValue ("@CreatedAtUtc", createdAtUtc.ToRoundtripString ());
                xCommand.Parameters.AddWithValue ("@Key", key);
                xCommand.Parameters.AddWithValue ("@Value", value);

                xCommand.ExecuteNonQuery ();
            }

            xConnection.Close ();
        }
    }
}
