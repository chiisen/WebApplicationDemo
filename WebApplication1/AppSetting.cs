namespace WebApplication1
{
    public class ISqlSettings
    {
        public string? Name { get; set; } = "MS-SQL";
        public string? Server { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
    }

    public class MsSqlSettings: ISqlSettings
    {
        public string? Database { get; set; }

        public string? TrustServerCertificate { get; set; }

    }

    public class IRedisSettings
    {
        public string? Name { get; set; } = "Redis";
    }

    public class RedisSettings : IRedisSettings
    {
        public string? Server { get; set; }
    }
}
