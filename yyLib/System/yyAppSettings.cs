using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public static class yyAppSettings
    {
        // Configuration in ASP.NET Core | Microsoft Learn
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/

        // ============================
        // 1. Detecting Configuration Changes
        // ============================
        // - When "reloadOnChange: true" is set, IConfiguration automatically reloads values from JSON files when modified.
        // - To detect changes in ASP.NET Core, use IOptionsMonitor<T> with an OnChange() event.
        // - For desktop applications (WPF/WinForms), use a background task that periodically checks IConfiguration values.

        // ============================
        // 2. Updating the UI Dynamically When Configuration Changes
        // ============================
        // - Use IOptionsMonitor<T>.OnChange() to listen for changes and update UI components dynamically.
        // - Example: When "FontFamily" changes in appsettings.json, update the UI theme or text elements.
        // - In desktop applications, a background polling mechanism may be needed since IConfiguration does not trigger events.

        // ============================
        // 3. Reloading IConfiguration Manually
        // ============================
        // - IConfiguration does not provide a direct "Reload()" method, but IConfigurationRoot does.
        // - Example:
        //   IConfigurationRoot config = (IConfigurationRoot)configuration;
        //   config.Reload(); // Manually reload configuration if needed.

        // ============================
        // 4. Modifying IConfiguration Dynamically
        // ============================
        // - IConfiguration is read-only; modifying values at runtime requires an InMemoryCollection() provider.
        // - Example:
        //   var configBuilder = new ConfigurationBuilder()
        //       .AddInMemoryCollection(new Dictionary<string, string> { { "UISettings:FontFamily", "Arial" } })
        //       .Build();
        //
        //   config["UISettings:FontFamily"] = "Comic Sans MS"; // Change at runtime (but not saved to JSON)

        // ============================
        // 5. Writing Changes Back to appsettings.json
        // ============================
        // - IConfiguration does NOT save changes back to JSON files automatically.
        // - To persist changes, manually read the JSON file, modify the values, and write it back.
        // - Example:
        //   var json = File.ReadAllText("appsettings.json");
        //   var jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        //   jsonObj["UISettings"] = new Dictionary<string, object> { { "FontFamily", "Times New Roman" } };
        //   File.WriteAllText("appsettings.json", JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true }));

        // ============================
        // 6. Best Practices
        // ============================
        // ✅ Use IOptionsMonitor<T> for real-time updates (ASP.NET Core).
        // ✅ Use polling mechanisms for WPF/WinForms apps.
        // ✅ To persist changes, manually update JSON and reload IConfiguration.
        // ✅ Use InMemoryCollection() for temporary changes that reset on restart.

        private static readonly Lazy <IConfiguration> _config = new (() =>
        {
            ConfigurationBuilder xBuilder = new ();

            xBuilder.SetBasePath (yyAppDirectory.Path);

            // appsettings.json: The default configuration file used by the app.
            // Contains settings that are common across all environments.

            xBuilder.AddJsonFile ("appsettings.json", optional: true, reloadOnChange: true);
#if DEBUG
            // appsettings.Development.json: Used when the app is running in a development environment.
            // This file contains development-specific settings (like detailed logging settings, local database connection strings, etc.) that override or supplement the base appsettings.json.

            xBuilder.AddJsonFile ("appsettings.Development.json", optional: true, reloadOnChange: true);

            // appsettings.Staging.json: Used in a staging environment, which is often a pre-production environment used for final testing before deployment to production.
            // This file can include settings closer to what is used in production, such as higher-level logging, staging database connections, etc.

            xBuilder.AddJsonFile ("appsettings.Staging.json", optional: true, reloadOnChange: true);
#endif
            // appsettings.Production.json: Contains configurations specific to the production environment.
            // These settings override the base appsettings.json when the app is running in production.
            // This may include production-level settings such as database connection strings, API keys, logging levels, and other settings optimized for performance and security.

            xBuilder.AddJsonFile ("appsettings.Production.json", optional: true, reloadOnChange: true);

            return xBuilder.Build ();
        });

        public static IConfiguration Config => _config.Value;
    }
}
