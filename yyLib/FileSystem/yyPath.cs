namespace yyLib
{
    public static class yyPath
    {
        public static readonly char [] _separators = [yyPathSeparator.Windows, yyPathSeparator.Unix];

        public static char [] Separators => _separators;

        public static char GetAlternativeSeparator (char separator)
        {
            if (separator == yyPathSeparator.Windows)
                return yyPathSeparator.Unix;

            else if (separator == yyPathSeparator.Unix)
                return yyPathSeparator.Windows;

            else throw new ArgumentException ("Invalid separator.");
        }

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
