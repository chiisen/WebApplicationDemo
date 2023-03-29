using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Data;
using System.Text.Json;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastRedisController : ControllerBase
    {
        private readonly ILogger<WeatherForecastRedisController> _logger;

        private readonly RedisSettings _redis;
        private ConnectionMultiplexer _conn;
        private readonly string _key = "Summaries";

        public WeatherForecastRedisController(ILogger<WeatherForecastRedisController> logger, IRedisSettings redisSetting)
        {
            _logger = logger;

            _redis = (RedisSettings)redisSetting;
            _conn = ConnectionMultiplexer.Connect($"{_redis.Server}");
        }

        /// <summary>
        /// Redis 取得所有資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpGet(Name = "GetWeatherForecastRedis")]
        public IEnumerable<WeatherForecastRedis>? Get()
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 GetWeatherForecastRedis");

            IDatabase db_ = _conn.GetDatabase(0);
            RedisValue redisValue_ = db_.StringGet(_key);
            if(redisValue_.IsNullOrEmpty)
            {
                return null;
            }
            string jsonString_ = (string)redisValue_;
            var Summary_ = JsonSerializer.Deserialize<string[]>(jsonString_);
            if(Summary_ == null)
            {
                return null;
            }

            _logger.LogInformation($"{convertedUUID_} 🚥 🍳撈取 Redis 資料結束");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecastRedis
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summary_[Random.Shared.Next(Summary_.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// Redis 新增一筆資料
        /// </summary>
        /// <param name="json">新增的 json 格式資料</param>
        /// <returns>回傳執行結果</returns>
        /// <response code="200">新增成功</response>
        /// <response code="400">新增失敗</response>          
        [HttpPost(Name = "PostWeatherForecastRedis")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public string Post(string json)
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 PostWeatherForecastRedis");

            IDatabase db_ = _conn.GetDatabase(0);
            db_.StringSet(_key, json);

            _logger.LogInformation($"{convertedUUID_} 🚥 【{json}】 🌱新增資料成功");

            return $"【{json}】 新增成功";
        }

        /// <summary>
        /// Redis 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastRedis")]
        public string Delete(string key)
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 DeleteWeatherForecastRedis");

            IDatabase db_ = _conn.GetDatabase(0);
            db_.KeyDelete(key);
            
            _logger.LogInformation($"{convertedUUID_} 🚥 【{key}】 成功🔥刪除");

            return $"【{key}】 成功刪除";
        }

        [HttpPut("{weather}", Name = "PutWeatherForecastRedis")]
        public string Put(string weather, string weatherNew)
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 PutWeatherForecastRedis");

            IDatabase db_ = _conn.GetDatabase(0);
            RedisValue redisValue_ = db_.StringGet(_key);
            if (redisValue_.IsNullOrEmpty)
            {
                return $"【{weather}】 更新為 【{weatherNew}】 失敗";
            }
            string jsonString_ = (string)redisValue_;
            var Summary_ = JsonSerializer.Deserialize<string[]>(jsonString_);
            if (Summary_ == null)
            {
                return $"【{weather}】 更新為 【{weatherNew}】 失敗";
            }

            // 更新指定的天氣預報內容
            bool find_ = false;
            for (int i = 0; i < Summary_.Length; i++)
            {
                if (Summary_[i] == weather)
                {
                    Summary_[i] = weatherNew;

                    find_ = true;
                }
            }

            if (find_)
            {
                _logger.LogInformation($"{convertedUUID_} 🚥 🈶找到【{weather}】");
            }
            else
            {
                _logger.LogInformation($"{convertedUUID_} 🚥 🈚找到不到【{weather}】");
            }

            string SummaryJsonString_ = JsonSerializer.Serialize(Summary_);

            db_.StringSet(_key, SummaryJsonString_);

            _logger.LogInformation($"{convertedUUID_} 🚥 【{weather}】 ⚙更新為 【{weatherNew}】 成功");

            return $"【{weather}】 更新為 【{weatherNew}】 成功";
        }
    }
}