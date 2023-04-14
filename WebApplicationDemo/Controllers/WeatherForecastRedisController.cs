using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using WebApplicationDemo.Models;
using WebApplicationDemo.Models.AppSettings.CacheSettings;
using WebApplicationDemo.Models.Cache;
using WebApplicationDemo.Models.Redis;
using WebApplicationDemo.Services.Common;

namespace WebApplicationDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WeatherForecastRedisController : ControllerBase
    {
        private readonly ILogger<WeatherForecastRedisController> _logger;

        private readonly string _key = "Summaries";
        private ICacheService _cache;

        public WeatherForecastRedisController(ILogger<WeatherForecastRedisController> logger, ICacheService cache)
        {
            _logger = logger;

            _cache = cache;
        }

        /// <summary>
        /// Redis 取得所有資料
        /// </summary>
        /// <returns>回傳執行結果</returns>
        [HttpGet(Name = "GetWeatherForecastRedis")]
        public WeatherForecastRedis Get()
        {
            try
            {
                string convertedUUID_ = Guid.NewGuid().ToString();

                _logger.LogInformation($"{convertedUUID_} 🚥 收到 GetWeatherForecastRedis");

                var result_ = _cache.GetCacheKey(_key);
                if(result_.Item1 == false)
                {
                    _logger.LogError($"{convertedUUID_} 🚥 【{_key}】 🚫讀取失敗");
                    return new WeatherForecastRedis()
                    {
                        Data = Array.Empty<WeatherForecastData>(),
                        Code = (int)ResponseCode.Fail,
                        Message = ResponseMessageCode.Message[(int)ResponseCode.Fail]
                    };
                }

                string jsonString_ = result_.Item2;
                var Summary_ = JsonSerializer.Deserialize<string[]>(jsonString_);
                if (Summary_ == null)
                {
                    _logger.LogError($"{convertedUUID_} 🚥 【{_key}】 🚫字串不是JSON格式");
                    return new WeatherForecastRedis()
                    {
                        Data = Array.Empty<WeatherForecastData>(),
                        Code = (int)ResponseCode.Fail,
                        Message = ResponseMessageCode.Message[(int)ResponseCode.Fail]
                    };
                }

                _logger.LogInformation($"{convertedUUID_} 🚥 🍳撈取 Redis 資料結束");

                WeatherForecastData[] arr_ = Enumerable.Range(1, 5).Select(index => new WeatherForecastData
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summary_[Random.Shared.Next(Summary_.Length)]
                })
                .ToArray();

                WeatherForecastRedis redis_ = new() { Data = arr_ };
                return redis_;
            }
            catch (Exception ex)
            {
                var errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var errorFile = new StackTrace(ex, true).GetFrame(0).GetFileName();
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);

                return new WeatherForecastRedis()
                {
                    Data = Array.Empty<WeatherForecastData>(),
                    Code = (int)ResponseCode.Fail,
                    Message = ResponseMessageCode.Message[(int)ResponseCode.Fail]
                }; ;


            }
        }

        /// <summary>
        /// Redis 新增一筆資料
        /// </summary>
        /// <param name="json">新增的 json 格式資料</param>
        /// <returns>回傳執行結果</returns>
        [HttpPost(Name = "PostWeatherForecastRedis")]
        public string Post(string json)
        {
            try
            {
                string convertedUUID_ = Guid.NewGuid().ToString();

                _logger.LogInformation($"{convertedUUID_} 🚥 收到 PostWeatherForecastRedis");

                if(_cache.SetCacheKey(_key, json) == false)
                {
                    _logger.LogError($"{convertedUUID_} 🚥 【{json}】 🚫新增失敗");
                    return $"【{json}】 新增失敗";
                }

                _logger.LogInformation($"{convertedUUID_} 🚥 【{json}】 🌱新增成功");
                return $"【{json}】 新增成功";
            }
            catch (Exception ex)
            {
                var errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var errorFile = new StackTrace(ex, true).GetFrame(0).GetFileName();
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
                return "發生例外";
            }
        }

        /// <summary>
        /// Redis 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastRedis")]
        public string Delete(string key)
        {
            try
            {
                string convertedUUID_ = Guid.NewGuid().ToString();

                _logger.LogInformation($"{convertedUUID_} 🚥 收到 DeleteWeatherForecastRedis");

                if(!_cache.KeyDelete(key))
                {
                    _logger.LogError($"{convertedUUID_} 🚥 【{key}】 🚫刪除失敗");
                    return $"【{key}】 刪除失敗";
                }

                _logger.LogInformation($"{convertedUUID_} 🚥 【{key}】 成功🔥刪除");
                return $"【{key}】 成功刪除";
            }
            catch (Exception ex)
            {
                var errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var errorFile = new StackTrace(ex, true).GetFrame(0).GetFileName();
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
                return "發生例外";
            }
        }

        /// <summary>
        /// Redis 指定更新 Redis Key
        /// </summary>
        /// <param name="weather"></param>
        /// <param name="weatherNew"></param>
        /// <returns></returns>
        [HttpPut("{weather}", Name = "PutWeatherForecastRedis")]
        public string Put(string weather, string weatherNew)
        {
            try
            {
                string convertedUUID_ = Guid.NewGuid().ToString();

                _logger.LogInformation($"{convertedUUID_} 🚥 收到 PutWeatherForecastRedis");

                var result_ = _cache.GetCacheKey(_key);
                if (result_.Item1 == false)
                {
                    _logger.LogError($"{convertedUUID_} 🚥 【{weather}】 更新為 【{weatherNew}】 🚫失敗");
                    return $"【{weather}】 更新為 【{weatherNew}】 失敗";
                }

                string jsonString_ = result_.Item2;
                var summary_ = JsonSerializer.Deserialize<string[]>(jsonString_);
                if (summary_ == null)
                {
                    return $"【{weather}】 更新為 【{weatherNew}】 🚫失敗";
                }

                // 更新指定的天氣預報內容
                bool find_ = false;
                for (int i = 0; i < summary_.Length; i++)
                {
                    if (summary_[i] == weather)
                    {
                        summary_[i] = weatherNew;

                        find_ = true;
                    }
                }

                if (find_)
                {
                    _logger.LogInformation($"{convertedUUID_} 🚥 🈶找到【{weather}】");
                }
                else
                {
                    _logger.LogError($"{convertedUUID_} 🚥 🈚找到不到【{weather}】");
                }

                string summaryJsonString_ = JsonSerializer.Serialize(summary_);

                if (_cache.SetCacheKey(_key, summaryJsonString_) == false)
                {
                    _logger.LogError($"{convertedUUID_} 🚥 【{summaryJsonString_}】 🚫更新失敗");
                    return $"【{weather}】 更新為 【{weatherNew}】更新【{summaryJsonString_}】失敗";
                }

                _logger.LogInformation($"{convertedUUID_} 🚥 【{weather}】 ⚙更新為 【{weatherNew}】 成功");

                return $"【{weather}】 更新為 【{weatherNew}】 成功";
            }
            catch (Exception ex)
            {
                var errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var errorFile = new StackTrace(ex, true).GetFrame(0).GetFileName();
                _logger.LogError("exception EX : {ex}  MSG : {Message} Error Line : {errorFile}.{errorLine}", ex.GetType().FullName, ex.Message, errorFile, errorLine);
                return "";
            }

        }
    }
}