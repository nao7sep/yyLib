using System.Text;

namespace yyLib
{
    // todo: Could be a static class.

    public class yyUserSecretsLoader
    {
        public yyUserSecretsParser Parser { get; } = new ();

        // todo: Rewrite the summary.
        // todo: There may be yyUserSecretsModel.Default instead.
        // todo: Take encoding?

        /// <summary>
        /// 'filePaths' are tried in order.
        /// First file found is loaded.
        /// If it fails, an exception is thrown.
        /// </summary>
        public yyUserSecretsModel Load (string []? filePaths = null)
        {
            foreach (string xFilePath in filePaths ?? yyUserSecrets.DefaultFilePaths)
            {
                if (File.Exists (xFilePath))
                    return Parser.Parse (File.ReadAllText (xFilePath, Encoding.UTF8));
            }

            return new ();
        }
    }
}
