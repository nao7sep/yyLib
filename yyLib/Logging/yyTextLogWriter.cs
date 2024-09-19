using System.Text;

namespace yyLib
{
    public class yyTextLogWriter (string filePath, Encoding? encoding = null): yyLogWriter
    {
        private static readonly object _lock = new ();

        public string FilePath { get; init; } = filePath;

        public Encoding Encoding { get; init; } = encoding ?? Encoding.UTF8;

        public void Write (DateTime createdAtUtc, string key, string value)
        {
            lock (_lock)
            {
                StringBuilder xBuilder = new ();

                if (File.Exists (FilePath))
                    xBuilder.AppendLine ("----");

                xBuilder.AppendLine ($"UTC: {createdAtUtc.ToRoundtripString ()}");
                xBuilder.AppendLine ($"{key}: {value.TrimRedundantLines ()}"); // Auto trimmed.

                yyDirectory.CreateParent (FilePath);
                File.AppendAllText (FilePath, xBuilder.ToString (), Encoding);
            }
        }
    }
}
