namespace WebApplicationDemo.Models.AppSettings.SqlSettings
{
    public class MsSqlSettings : ISqlSettings
    {
        public string? Database { get; set; }

        public string? TrustServerCertificate { get; set; }

    }
}
