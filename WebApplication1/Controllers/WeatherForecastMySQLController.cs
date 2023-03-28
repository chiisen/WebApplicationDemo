using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastMySQLController : ControllerBase
    {
        private readonly ILogger<WeatherForecastMySQLController> _logger;

        private readonly MySqlConnectionStringBuilder _builder = new()
        {
            Server = "127.0.0.1",
            UserID = "dev",
            Password = "123456",
            Database = "dev",
        };

        private readonly string _field = "Summaries";

        public WeatherForecastMySQLController(ILogger<WeatherForecastMySQLController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecastMySQL")]
        public IEnumerable<WeatherForecastMySQL> Get()
        {
            _logger.LogInformation("收到 GetWeatherForecastMySQL");

            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM WeatherForecast;";
            /*
            command.CommandText = @"SELECT * FROM WeatherForecast WHERE Summaries = @Summaries;";
            command.Parameters.AddWithValue("@Summaries", "Cool");
            */

            // execute the command and read the results
            using var reader = command.ExecuteReader();
            List<string> Summaries_ = new();
            while (reader.Read())
            {
                var Sum_ = reader.GetString(_field);
                Summaries_.Add(Sum_);
            }


            return Enumerable.Range(1, 5).Select(index => new WeatherForecastMySQL
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries_[Random.Shared.Next(Summaries_.Count)]
            })
            .ToArray();
        }

        /// <summary>
        /// Redis 新增一筆資料
        /// </summary>
        /// <param name="weather">新增的 json 格式資料</param>
        /// <returns>回傳執行結果</returns>         
        [HttpPost(Name = "PostWeatherForecastMySQL")]
        public string Post(string weather)
        {
            _logger.LogInformation("收到 PostWeatherForecastMySQL");

            //在 WeatherForecast 資料表新增一筆資料
            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();
            string sql = $"INSERT INTO WeatherForecast ( `Summaries` ) VALUES ( '{weather}')";
            MySqlCommand cmd = new(sql, connection);
            int n = cmd.ExecuteNonQuery();

            return $"【{weather}】 新增 {n} 筆資料成功";
        }

        /// <summary>
        /// Redis 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastMySQL")]
        public string Delete(string key)
        {
            _logger.LogInformation("收到 DeleteWeatherForecastMySQL");

            //刪除 WeatherForecast 資料表中 Summaries 欄位值為指定的資料
            string sql = $"DELETE FROM WeatherForecast WHERE Summaries='{key}'";
            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();
            MySqlCommand cmd = new(sql, connection);
            int n = cmd.ExecuteNonQuery();

            return $"【{key}】 成功刪除";
        }

        [HttpPut("{weather}", Name = "PutWeatherForecastMySQL")]
        public string Put(string weather, string weatherNew)
        {
            _logger.LogInformation("收到 PutWeatherForecastMySQL");

            //將 WeatherForecast 資料表中修改 Summaries 欄位值為指定內容
            string sql = $"UPDATE WeatherForecast SET Summaries='{weatherNew}' WHERE Summaries='{weather}'";
            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();
            MySqlCommand cmd = new(sql, connection);
            int n = cmd.ExecuteNonQuery();

            return $"【{weather}】 更新為 【{weatherNew}】 共 {n} 筆資料成功";
        }
    }
}