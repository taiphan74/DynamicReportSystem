using System;
using System.IO;
using System.Text.Json;
using DynamicReportSystem.Models;

namespace DynamicReportSystem.Configuration
{
    public static class AppConfiguration
    {
        public static string GetConnectionString(string key = "DefaultConnection")
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("appsettings.json was not found in the application directory.", configPath);
            }

            var json = File.ReadAllText(configPath);
            var options = JsonSerializer.Deserialize<AppSettingsOptions>(json);

            if (options?.ConnectionStrings is null)
            {
                throw new InvalidOperationException("Missing 'ConnectionStrings' section in appsettings.json.");
            }

            var connectionString = key switch
            {
                "DefaultConnection" => options.ConnectionStrings.DefaultConnection,
                _ => throw new InvalidOperationException($"Connection string key '{key}' is not supported.")
            };

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"ConnectionStrings:{key} is empty.");
            }

            return connectionString;
        }
    }
}
