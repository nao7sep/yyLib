namespace yyLib
{
    // A simple class to represent the application directory rather than the application itself.

    public static class yyApplicationDirectory
    {
        public static Lazy <string> _path { get; } = new (() => AppContext.BaseDirectory);

        public static string Path => _path.Value;

        public static string MapPath (string relativePath)
        {
            if (string.IsNullOrWhiteSpace (relativePath) || System.IO.Path.IsPathFullyQualified (relativePath))
                throw new yyArgumentException ($"'{nameof (relativePath)}' is invalid: {relativePath}");

            return System.IO.Path.Join (Path, relativePath);
        }
    }
}
