using System.Text;

namespace yyLib
{
    public class yySimpleLogTextWriter
    {
        public string FilePath { get; private set; }

        public Encoding Encoding { get; private set; }

        public yySimpleLogTextWriter (string filePath, Encoding? encoding = null)
        {
            FilePath = filePath;
            Encoding = encoding ?? Encoding.UTF8;
        }

        public void Write (DateTime creationUtc, string key, string value)
        {
            StringBuilder xBuilder = new ();

            if (File.Exists (FilePath))
                xBuilder.AppendLine ("----");

            xBuilder.AppendLine ($"Time: {creationUtc.ToRoundtripString ()}");
            xBuilder.AppendLine ($"{key}: {value.TrimWhiteSpaceLines ()}"); // Auto trimmed.

            yyDirectory.CreateParent (FilePath);
            File.AppendAllText (FilePath, xBuilder.ToString (), Encoding);
        }
    }
}
