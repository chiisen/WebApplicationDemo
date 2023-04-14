namespace WebApplicationDemo.Services.Common
{
    public interface ICacheService
    {
        Tuple<bool, string> GetCacheKey(string key);
        bool SetCacheKey(string key, string value);

        bool KeyDelete(string key);
    }
}
