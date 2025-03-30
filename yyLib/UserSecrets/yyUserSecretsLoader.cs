using System.Text;

namespace yyLib
{
    public static class yyUserSecretsLoader
    {
        /// <summary>
        /// 'filePaths' are tried in order.
        /// First file found is loaded.
        /// If loading the first one fails, an exception is thrown.
        /// </summary>
        public static yyUserSecrets Load (string [] filePaths, Encoding? encoding = null)
        {
            foreach (string xFilePath in filePaths)
            {
                if (string.IsNullOrWhiteSpace (xFilePath) || Path.IsPathFullyQualified (xFilePath) == false)
                    throw new yyArgumentException ($"'{nameof (filePaths)}' is invalid: {xFilePath.GetVisibleString ()}");

                if (File.Exists (xFilePath))
                    return yyUserSecretsParser.Parse (File.ReadAllText (xFilePath, encoding.OrDefaultEncoding ()));
            }

            // If no file is found, it's not an error.
            // If one of the files exists and is empty, it'd be the same result.
            return new ();
        }
    }
}
