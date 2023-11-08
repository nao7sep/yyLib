using System.Text;

namespace yyLib
{
    public class yyUserSecretsLoader
    {
        public yyUserSecretsParser Parser { get; private set; }

        public yyUserSecretsLoader () => Parser = new yyUserSecretsParser ();

        /// <summary>
        /// filePaths are tried in order, first file found is loaded. If it fails, an exception is thrown.
        /// </summary>
        public yyUserSecretsModel Load (string []? filePaths = null)
        {
            foreach (string xFilePath in filePaths ?? yyUserSecretsDefaultValues.FilePaths)
            {
                if (File.Exists (xFilePath))
                    return Parser.Parse (File.ReadAllText (xFilePath, Encoding.UTF8));
            }

            return yyUserSecretsModel.Empty;
        }
    }
}
