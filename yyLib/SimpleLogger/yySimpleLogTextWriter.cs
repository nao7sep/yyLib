using System.Text;

namespace yyLib
{
    public class yySimpleLogTextWriter (string filePath, Encoding? encoding = null)
    {
        public string FilePath { get; private set; } = filePath;

        public Encoding Encoding { get; private set; } = encoding ?? Encoding.UTF8;

        public void Write (DateTime creationUtc, string key, string value)
        {
            StringBuilder xBuilder = new ();

            if (File.Exists (FilePath))
                xBuilder.AppendLine ("----");

            xBuilder.AppendLine ($"Time: {creationUtc.ToRoundtripString ()}");
            xBuilder.AppendLine ($"{key}: {value.TrimLines ()}"); // Auto trimmed.

            yyDirectory.CreateParent (FilePath);
            File.AppendAllText (FilePath, xBuilder.ToString (), Encoding);
        }
    }
}
