namespace DynamicReportSystem.Models
{
    public class AppSettingsOptions
    {
        public ConnectionStringsOptions ConnectionStrings { get; set; } = new();
    }

    public class ConnectionStringsOptions
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }
}
