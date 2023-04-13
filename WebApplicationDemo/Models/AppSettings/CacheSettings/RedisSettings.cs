using WebApplicationDemo.Models.AppSettings.CacheSettings;

namespace WebApplicationDemo.Models.AppSettings.RedisSettings
{
    public class RedisSettings : ICacheSettings
    {
        public string? Server { get; set; }
    }
}
