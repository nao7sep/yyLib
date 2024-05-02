using System.Reflection;

namespace yyLib
{
    public static class yyAppAssembly
    {
        private static readonly Lazy <Assembly?> _assembly = new (() => Assembly.GetEntryAssembly ());

        public static Assembly? Assembly => _assembly.Value;

        private static readonly Lazy <string?> _location = new (() => Assembly?.Location);

        public static string? Location => _location.Value;

        private static readonly Lazy <FileInfo?> _assemblyFile = new (() => Location != null ? new FileInfo (Location) : null);

        public static FileInfo? AssemblyFile => _assemblyFile.Value;
    }
}
