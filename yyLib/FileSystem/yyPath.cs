namespace yyLib
{
    public static class yyPath
    {
        private static readonly char [] _separators = [yyPathSeparators.Nt, yyPathSeparators.Posix];

        public static char [] Separators => _separators;

        private static readonly Lazy <char> _defaultSeparator = new (() =>
        {
            if (yyEnvironment.IsNt)
                return yyPathSeparators.Nt;

            else if (yyEnvironment.IsPosix)
                return yyPathSeparators.Posix;

            else throw new yyNotSupportedException ("Unsupported operating system.");
        });

        public static char DefaultSeparator => _defaultSeparator.Value;

        public static char GetAlternativeSeparator (char separator)
        {
            if (separator == yyPathSeparators.Nt)
                return yyPathSeparators.Posix;

            else if (separator == yyPathSeparators.Posix)
                return yyPathSeparators.Nt;

            else throw new ArgumentException ("Invalid separator.");
        }

        private static readonly Lazy <char> _alternativeSeparator = new (() => GetAlternativeSeparator (DefaultSeparator));

        public static char AlternativeSeparator => _alternativeSeparator.Value;

        public static string NormalizeSeparators (string path, char separator) =>
            path.Replace (GetAlternativeSeparator (separator), separator);

        public static string Join (char separator, params string [] paths)
        {
            if (paths.Length < 2)
                throw new yyArgumentException ("At least two paths are required.");

            List <string> xTrimmedPaths = new ();

            xTrimmedPaths.Add (paths [0].TrimEnd (Separators));
            xTrimmedPaths.AddRange (paths [1..^1].Select (x => x.Trim (Separators)));
            xTrimmedPaths.Add (paths [^1].TrimStart (Separators));

            if (xTrimmedPaths.Any (x => string.IsNullOrEmpty (x)))
                throw new yyArgumentException ("Empty paths are not allowed.");

            string xJoinedPath = string.Join (separator, xTrimmedPaths);

            return NormalizeSeparators (xJoinedPath, separator);
        }

        public static string GetAbsolutePath (string basePath, string relativePath, char separator)
        {
            if (Path.IsPathFullyQualified (basePath) == false)
                throw new yyArgumentException ("The base path must be fully qualified.");

            if (Path.IsPathFullyQualified (relativePath))
                throw new yyArgumentException ("The relative path must be relative.");

            return Join (separator, basePath, relativePath);
        }
    }
}
