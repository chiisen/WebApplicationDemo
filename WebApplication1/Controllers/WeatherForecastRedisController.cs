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

        private readonly ConnectionMultiplexer _conn = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        private readonly string _key = "Summaries";

        public WeatherForecastRedisController(ILogger<WeatherForecastRedisController> logger)
        {
            _logger = logger;

            _logger.LogInformation("初始化 GetWeatherForecastRedis");
        }

        /// <summary>
        /// Redis 取得所有資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpGet(Name = "GetWeatherForecastRedis")]
        public IEnumerable<WeatherForecastRedis>? Get()
        {
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
            IDatabase db_ = _conn.GetDatabase(0);
            db_.StringSet(_key, json);

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
            IDatabase db_ = _conn.GetDatabase(0);
            db_.KeyDelete(key);
            

            return $"【{key}】 成功刪除";
        }

        [HttpPut("{weather}", Name = "PutWeatherForecastRedis")]
        public string Put(string weather, string weatherNew)
        {
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
            for(int i = 0; i < Summary_.Length; i++)
            {
                if(Summary_[i] == weather)
                {
                    Summary_[i] = weatherNew;
                }
            }

            string SummaryJsonString_ = JsonSerializer.Serialize(Summary_);

            db_.StringSet(_key, SummaryJsonString_);


            return $"【{weather}】 更新為 【{weatherNew}】 成功";
        }
    }
}