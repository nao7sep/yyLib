﻿using System.Reflection;

namespace yyLib
{
    public static class yyApplicationAssembly
    {
        private static readonly Lazy <Assembly?> _assembly = new (() => Assembly.GetEntryAssembly ());

        public static Assembly? Assembly => _assembly.Value;

        private static readonly Lazy <string?> _location = new (() => Assembly?.Location);

        public static string? Location => _location.Value;

        private static readonly Lazy <string?> _directoryPath = new (() => Path.GetDirectoryName (Location));

        public static string? DirectoryPath => _directoryPath.Value;
    }
}
