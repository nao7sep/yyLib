using System.Reflection;

namespace yyLib
{
    public static class yyLibraryAssembly
    {
        private static readonly Lazy <Assembly> _assembly = new (() => Assembly.GetExecutingAssembly ());

        public static Assembly Assembly => _assembly.Value;

        private static readonly Lazy <string> _location = new (() => Assembly.Location);

        public static string Location => _location.Value;

        private static readonly Lazy <FileInfo> _assemblyFile = new (() => new FileInfo (Location));

        public static FileInfo AssemblyFile => _assemblyFile.Value;
    }
}
