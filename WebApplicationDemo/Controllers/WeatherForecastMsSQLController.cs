using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApplicationDemo.Models.AppSettings.SqlSettings;
using WebApplicationDemo.Models.SQL;

namespace WebApplicationDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WeatherForecastMsSQLController : ControllerBase
    {
        private readonly ILogger<WeatherForecastMsSQLController> _logger;

        private readonly MsSqlSettings _msSql;

        private readonly string _connectionString;

        private const string _field = "Summaries";

        public WeatherForecastMsSQLController(ILogger<WeatherForecastMsSQLController> logger, ISqlSettings msSqlSetting)
        {
            _logger = logger;

            _msSql = (MsSqlSettings)msSqlSetting;
            _connectionString = $"Server={_msSql.Server};Database={_msSql.Database};User={_msSql.User};Password={_msSql.Password};TrustServerCertificate={_msSql.TrustServerCertificate}";
        }

        /// <summary>
        /// 取得 資料庫 的資料
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetWeatherForecastMsSQL")]
        [Produces("application/json")]
        public IEnumerable<WeatherForecastMsSQL> Get()
        {
            var convertedUuid = Guid.NewGuid().ToString();

            _logger.LogInformation($"{convertedUuid} 🚥 收到 GetWeatherForecastMsSQL");

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.Connection.Open();
            cmd.CommandText = @"SELECT * FROM WeatherForecast;";

            List<string?> summaries = new();
            using (conn)
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var oneField = reader[_field];
                    if (oneField is null)
                    {
                        continue;
                    }
                    var sum = Convert.ToString(oneField);
                    summaries.Add(sum);

                    _logger.LogInformation($"{convertedUuid} 🚥 🍳撈取 MS-SQL 資料為 {sum}");
                }
            }

            cmd.Connection.Close();

            _logger.LogInformation($"{convertedUuid} 🚥 🍳撈取 MS-SQL 資料結束");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecastMsSQL
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Count)]
            })
            .ToArray();
        }

        /// <summary>
        /// 資料庫 新增一筆資料
        /// </summary>
        /// <param name="weather">新增的 json 格式資料</param>
        /// <returns>回傳執行結果</returns>         
        [HttpPost(Name = "PostWeatherForecastMsSQL")]
        [Produces("application/json")]
        public string Post(string weather)
        {
            var convertedUuid = Guid.NewGuid().ToString();

            _logger.LogInformation($"{convertedUuid} 🚥 收到 PostWeatherForecastMsSQL");

            //在 WeatherForecast 資料表新增一筆資料
            using SqlConnection conn_ = new(_connectionString);

            using var cmd_ = conn_.CreateCommand();
            cmd_.Connection.Open();

            // 取得新增資料後自動產生的 id
            cmd_.CommandText = $"INSERT INTO WeatherForecast ( Summaries ) VALUES ( @weather )";
            cmd_.Parameters.AddWithValue("@weather", weather);
            cmd_.ExecuteScalar();
            cmd_.Connection.Close();

            _logger.LogInformation($"{convertedUuid} 🚥 【{weather}】 🌱新增資料成功");

            return $"【{weather}】 新增資料成功";
        }

        /// <summary>
        /// 資料庫 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastMsSQL")]
        [Produces("application/json")]
        public string Delete(string key)
        {
            var convertedUuid = Guid.NewGuid().ToString();

            _logger.LogInformation($"{convertedUuid} 🚥 收到 DeleteWeatherForecastMsSQL");

            //刪除 WeatherForecast 資料表中 Summaries 欄位值為指定的資料
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.Connection.Open();
            cmd.CommandText = "DELETE FROM WeatherForecast WHERE Summaries=@key";
            cmd.Parameters.AddWithValue("@key", key);

            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            _logger.LogInformation($"{convertedUuid} 🚥 【{key}】 成功🔥刪除");

            return $"【{key}】 成功刪除";
        }

        /// <summary>
        /// 資料庫 修改一筆資料
        /// </summary>
        /// <param name="weather"></param>
        /// <param name="weatherNew"></param>
        /// <returns></returns>
        [HttpPut("{weather}", Name = "PutWeatherForecastMsSQL")]
        [Produces("application/json")]
        public string Put(string weather, string weatherNew)
        {
            string convertedUUID_ = Guid.NewGuid().ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 PutWeatherForecastMsSQL");

            //將 WeatherForecast 資料表中修改 Summaries 欄位值為指定內容
            using SqlConnection conn_ = new(_connectionString);
            using SqlCommand cmd_ = conn_.CreateCommand();
            cmd_.Connection.Open();
            cmd_.CommandText = "UPDATE WeatherForecast SET Summaries=@weatherNew WHERE Summaries=@weather";
            cmd_.Parameters.AddWithValue("@weatherNew", weatherNew);
            cmd_.Parameters.AddWithValue("@weather", weather);
            cmd_.ExecuteNonQuery();
            cmd_.Connection.Close();

            _logger.LogInformation($"{convertedUUID_} 🚥 【{weather}】 ⚙更新為 【{weatherNew}】 成功");

            return $"【{weather}】 更新為 【{weatherNew}】 成功";
        }
    }
}