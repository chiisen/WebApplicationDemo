namespace WebApplicationDemo.Models.AppSettings.SqlSettings
{
    public class MsSqlSettings : ISqlSettings
    {
        public string? Database { get; set; }

        /// <summary>
        /// SQL Server 安裝時已預設支援加密連線，但預設用的 TLS 憑證是自我簽署憑證(Self-Signed Certificate)，故要勾選 Trust server
        /// certificate，否則會因無法驗證憑證有效性出現錯誤 一般都要勾選(true)
        /// </summary>
        public string? TrustServerCertificate { get; set; }

    }
}
