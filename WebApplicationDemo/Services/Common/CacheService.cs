using StackExchange.Redis;
using System.Diagnostics;
using WebApplicationDemo.Models.AppSettings.CacheSettings;
using WebApplicationDemo.Models.AppSettings.RedisSettings;

namespace WebApplicationDemo.Services.Common
{
    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;

        private readonly RedisSettings? _redis;
        private readonly ConnectionMultiplexer? _conn;
        private readonly IDatabase? _db;

        public CacheService(ILogger<CacheService> logger, ICacheSettings redisSetting)
        {
            _logger = logger;
            try
            {
                _redis = (RedisSettings)redisSetting;
                var server_ = _redis.Server;

                _logger.LogInformation($"Redis 開始連線網址: {server_}");
                _conn = ConnectionMultiplexer.Connect($"{server_}");
                _logger.LogInformation($"Redis 連線網址: {server_} 成功!");
                _db = _conn.GetDatabase(0);
            }
            catch (Exception ex)
            {
                var frame = new StackTrace(ex, true).GetFrame(0);
                var errorLine = frame?.GetFileLineNumber() ?? 0;
                var errorFile = frame?.GetFileName() ?? "unknown";
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
            }
        }
        public Tuple<bool, string> GetCacheKey(string key)
        {
            try
            {
                if (_db is null)
                {
                    _logger.LogError("Redis 無法連線!");
                    return Tuple.Create(false, "Redis 無法連線!");
                }
                var redisValue = _db.StringGet(key);
                if (redisValue.IsNullOrEmpty)
                {
                    return Tuple.Create(false, $"{key} 沒有填值");
                }
                var jsonString = (string?)redisValue ?? string.Empty;
                return Tuple.Create(true, jsonString);
            }
            catch (Exception ex)
            {
                var frame = new StackTrace(ex, true).GetFrame(0);
                var errorLine = frame?.GetFileLineNumber() ?? 0;
                var errorFile = frame?.GetFileName() ?? "unknown";
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
                return Tuple.Create(false, "執行發生例外!");
            }
        }

        public bool SetCacheKey(string key, string value)
        {
            try
            {
                if (_db is null)
                {
                    _logger.LogError("Redis 無法連線!");
                    return false;
                }
                _db.StringSet(key, value);
                return true;
            }
            catch (Exception ex)
            {
                var frame = new StackTrace(ex, true).GetFrame(0);
                var errorLine = frame?.GetFileLineNumber() ?? 0;
                var errorFile = frame?.GetFileName() ?? "unknown";
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
                return false;
            }
        }

        public bool KeyDelete(string key)
        {
            try
            {
                if (_db is null)
                {
                    _logger.LogError("Redis 無法連線!");
                    return false;
                }
                _db.KeyDelete(key);
                return true;
            }
            catch (Exception ex)
            {
                var frame = new StackTrace(ex, true).GetFrame(0);
                var errorLine = frame?.GetFileLineNumber() ?? 0;
                var errorFile = frame?.GetFileName() ?? "unknown";
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
                return false;
            }
        }
    }
}
