namespace yyLib
{
    public static class yyAppDirectory
    {
        private static readonly Lazy <string> _path = new (AppContext.BaseDirectory);

        public static string Path => _path.Value;

        public static string MapPath (string relativePath, char separator, bool normalize = true)
        {
            if (string.IsNullOrWhiteSpace (relativePath) || System.IO.Path.IsPathFullyQualified (relativePath))
                throw new yyArgumentException ($"'{nameof (relativePath)}' is invalid: {relativePath.GetVisibleString ()}");

            return yyPath.Join (separator, normalize, Path, relativePath);
        }

        public static string MapPath (string relativePath, bool normalize = true) =>
            MapPath (relativePath, yyPath.DefaultSeparator, normalize);
    }
}
