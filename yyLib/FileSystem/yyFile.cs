namespace yyLib
{
    public static class yyFile
    {
        /// <summary>
        /// Creates file if it does not exist.
        /// Parent directory is created automatically.
        /// </summary>
        public static void Create (string path)
        {
            if (string.IsNullOrWhiteSpace (path) || Path.IsPathFullyQualified (path) == false)
                throw new yyArgumentException ($"'{nameof (path)}' is invalid: {path.GetVisibleString ()}");

            if (File.Exists (path) == false)
            {
                yyDirectory.CreateParent (path);
                File.Create (path).Close ();
            }
        }
    }
}
