using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public static class yyAppSettings
    {
        // Configuration in ASP.NET Core | Microsoft Learn
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/

        // https://github.com/nao7sep/Resources/blob/main/Documents/AI-Generated%20Notes/Configuration%20in%20ASP.NET%20Core%20-%20Managing%20and%20Detecting%20Changes.md

        private static readonly Lazy<IConfiguration> _settings = new (() =>
        {
            ConfigurationBuilder xBuilder = new ();

            // Set the base path for the configuration files to the application's directory.
            xBuilder.SetBasePath (yyAppDirectory.Path);

            // Load the default configuration file, AppSettings.json.
            // This file contains settings that are common across all environments.
            xBuilder.AddJsonFile ("AppSettings.json", optional: true, reloadOnChange: true);
#if DEBUG
            // Load the development-specific configuration file, AppSettings.Development.json.
            // This file contains settings specific to the development environment, such as detailed logging and local database connections.
            xBuilder.AddJsonFile ("AppSettings.Development.json", optional: true, reloadOnChange: true);

            // Load the staging-specific configuration file, AppSettings.Staging.json.
            // This file is used in a staging environment, which is a pre-production environment for final testing.
            // It may include settings similar to production, like higher-level logging and staging database connections.
            xBuilder.AddJsonFile ("AppSettings.Staging.json", optional: true, reloadOnChange: true);
#endif
            // Load the production-specific configuration file, AppSettings.Production.json.
            // This file contains settings specific to the production environment, such as production-level database connections and API keys.
            xBuilder.AddJsonFile ("AppSettings.Production.json", optional: true, reloadOnChange: true);

            return xBuilder.Build ();
        });

        public static IConfiguration Settings => _settings.Value;
    }
}
