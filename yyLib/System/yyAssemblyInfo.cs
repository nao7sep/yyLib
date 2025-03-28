using System.Reflection;

namespace yyLib
{
    public class yyAssemblyInfo
    {
        public Assembly Assembly { get; private set; }

        public string Location => Assembly.Location;

        /// <summary>
        /// Provides a string representation of the assembly's full identity,
        /// including its name, version, culture, and public key token.
        /// </summary>
        public string? FullName => Assembly.FullName;

        // Lazy properties are initialized in the constructor because they rely on the Assembly property,
        // which is not available at the time of class instantiation.

        private readonly Lazy<string?> _assemblyName;

        /// <summary>
        /// Represents the name of the assembly used for the output file.
        /// For the display name, refer to AssemblyTitle.
        /// </summary>
        public string? AssemblyName => _assemblyName.Value;

        private readonly Lazy<string?> _assemblyTitle;

        /// <summary>
        /// Represents the display name of the assembly.
        /// Use this instead of AssemblyName, which is for the output file.
        /// </summary>
        public string? AssemblyTitle => _assemblyTitle.Value ?? AssemblyName;

        private readonly Lazy<string?> _company;

        public string? Company => _company.Value;

        private readonly Lazy<string?> _product;

        public string? Product => _product.Value;

        private readonly Lazy<string?> _description;

        public string? Description => _description.Value;

        private readonly Lazy<string?> _copyright;

        public string? Copyright => _copyright.Value;

        private readonly Lazy<string?> _configuration;

        /// <summary>
        /// The value may differ from the .csproj file due to compiler overrides.
        /// </summary>
        public string? Configuration => _configuration.Value;

        // Handling versions can be complex.

        // "Version" was initially intended for NuGet packages.
        // Some developers mistakenly thought it was a new general-purpose attribute.
        // The correct attribute for the assembly's version is "AssemblyVersion".
        // For convenience, .NET allows "Version" and "AssemblyVersion" to be used interchangeably.

        // AssemblyVersionAttribute may not function as expected.
        // Using Assembly.GetName().Version?.ToString() is a straightforward way to retrieve "Version" and "AssemblyVersion".
        // If both attributes are specified, this method returns "AssemblyVersion".
        // To obtain "Version" in this case, parsing the .csproj file might be necessary.

        // "Version" is more intuitive than "AssemblyVersion".
        // It is beneficial when releasing the assembly as a NuGet package, as it avoids updating two version strings.
        // Best practice: Specify "Version" and refer to the AssemblyVersion property programmatically.

        private readonly Lazy<Version?> _version;

        /// <summary>
        /// Represents the version of the NuGet package.
        /// For the assembly's version, refer to AssemblyVersion.
        /// </summary>
        public Version? Version => _version.Value;

        private readonly Lazy<Version?> _assemblyVersion;

        /// <summary>
        /// Represents the version of the assembly.
        /// Use this instead of Version, which is for the NuGet package.
        /// </summary>
        public Version? AssemblyVersion => _assemblyVersion.Value;

        private readonly Lazy<string?> _fileVersion;

        public string? FileVersion => _fileVersion.Value;

        private readonly Lazy<string?> _informationalVersion;

        /// <summary>
        /// Provides a detailed version string, including commit hash.
        /// Example: 0.1+03073b01cc6d77db9dfaab1b7411d7a940e54eb2
        /// </summary>
        public string? InformationalVersion => _informationalVersion.Value;

        // The constructor sets up the yyAssemblyInfo class with a specific Assembly instance,
        // ensuring the Assembly property is immutable after initialization.
        // This design choice maintains the consistency and integrity of the metadata extracted from the assembly.
        //
        // Unlike classes with self-updating properties via setters, which are suitable for scenarios like JSON serialization,
        // yyAssemblyInfo prioritizes immutability to prevent confusion and misuse.
        //
        // Lazy initialization is employed for non-essential properties to enhance performance and resource efficiency.
        // Properties such as Configuration or Description are computed only when accessed, ensuring optimal operation when these properties are not required.

        public yyAssemblyInfo (Assembly assembly)
        {
            Assembly = assembly;

            _assemblyName = new (() => Assembly.GetName ().Name);
            _assemblyTitle = new (() => Assembly.GetCustomAttribute<AssemblyTitleAttribute> ()?.Title);
            _company = new (() => Assembly.GetCustomAttribute<AssemblyCompanyAttribute> ()?.Company);
            _product = new (() => Assembly.GetCustomAttribute<AssemblyProductAttribute> ()?.Product);
            _description = new (() => Assembly.GetCustomAttribute<AssemblyDescriptionAttribute> ()?.Description);
            _copyright = new (() => Assembly.GetCustomAttribute<AssemblyCopyrightAttribute> ()?.Copyright);
            _configuration = new (() => Assembly.GetCustomAttribute<AssemblyConfigurationAttribute> ()?.Configuration);
            _version = new (() => Assembly.GetName ().Version);
            _assemblyVersion = new (() => Assembly.GetName ().Version);
            _fileVersion = new (() => Assembly.GetCustomAttribute<AssemblyFileVersionAttribute> ()?.Version);
            _informationalVersion = new (() => Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute> ()?.InformationalVersion);
        }

        private static readonly Lazy<yyAssemblyInfo?> _appAssembly = new (() =>
        {
            Assembly? xAssembly = Assembly.GetEntryAssembly ();

            if (xAssembly != null)
                return new (xAssembly);

            return null;
        });

        public static yyAssemblyInfo? AppAssembly => _appAssembly.Value;

        private static readonly Lazy<yyAssemblyInfo> _libAssembly = new (() =>
        {
            Assembly xAssembly = Assembly.GetExecutingAssembly ();
            return new (xAssembly);
        });

        public static yyAssemblyInfo LibAssembly => _libAssembly.Value;
    }
}
