namespace yyLib
{
    public static class yyDirectory
    {
        /// <summary>
        /// Creates directory if it does not exist.
        /// </summary>
        public static void Create (string path)
        {
            if (Directory.Exists (path) == false)
                Directory.CreateDirectory (path);
        }

        /// <summary>
        /// Creates parent directory if it doesnt exist.
        /// Can take paths like "C:\"; nothing will happen.
        /// </summary>
        public static void CreateParent (string path) // todo: Test all kinds.
        {
            // Straightforward implementation:

            // Path.GetDirectoryName returns null if 'path' is null or it's a root directory.
            // No point to specify null as 'path' and call this method.

            if (path == null || Path.IsPathFullyQualified (path) == false)
                throw new yyArgumentException (yyMessage.Create ($"'{nameof (path)}' is invalid."));
            // todo: Output path.
            // todo: Search for all "yyMessage.Create".

            string? xParentDirectoryPath = Path.GetDirectoryName (path);

            // Root directory like "C:\".
            // It would always exist.

            if (xParentDirectoryPath == null)
                return;

            // Relative path.
            // After Path.IsPathFullyQualified, probably redundant.

            if (xParentDirectoryPath == string.Empty)
                throw new yyArgumentException (yyMessage.Create ($"'{nameof (path)}' is invalid."));
            // todo: Output path.

            if (Directory.Exists (xParentDirectoryPath) == false)
                Directory.CreateDirectory (xParentDirectoryPath);
        }
    }
}
