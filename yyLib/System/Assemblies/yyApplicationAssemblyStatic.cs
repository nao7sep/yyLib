using System.Reflection;

namespace yyLib
{
    public static class yyApplicationAssembly
    {
        private static Lazy <Assembly?> _assembly = new Lazy <Assembly?> (() => Assembly.GetEntryAssembly ());

        public static Assembly? Assembly { get => _assembly.Value; }

        private static Lazy <string?> _location = new Lazy <string?> (() => Assembly?.Location);

        public static string? Location { get => _location.Value; }
    }
}
