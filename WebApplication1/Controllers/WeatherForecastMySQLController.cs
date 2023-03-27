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

        private readonly string _key = "Summaries";

        public WeatherForecastMySQLController(ILogger<WeatherForecastMySQLController> logger)
        {
            _logger = logger;

            _logger.LogInformation("初始化 WeatherForecastMySQL");
        }

        [HttpGet(Name = "GetWeatherForecastMySQL")]
        public IEnumerable<WeatherForecastMySQL> Get()
        {
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
                var Sum_ = reader.GetString(_key);
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
        /// <param name="json">新增的 json 格式資料</param>
        /// <returns>回傳執行結果</returns>         
        [HttpPost(Name = "PostWeatherForecastMySQL")]
        public string Post(string json)
        {
            //在 WeatherForecast 資料表新增一筆資料
            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();
            string sql = $"INSERT INTO WeatherForecast ( `Summaries` ) VALUES ( '{json}')";
            MySqlCommand cmd = new(sql, connection);
            int n = cmd.ExecuteNonQuery();

            return $"【{json}】 新增 {n} 筆資料成功";
        }

        /// <summary>
        /// Redis 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastMySQL")]
        public string Delete(string key)
        {
            //刪除 WeatherForecast 資料表中 Summaries 欄位值為指定的資料
            string sql = $"DELETE FROM WeatherForecast WHERE Summaries='{key}'";
            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();
            MySqlCommand cmd = new(sql, connection);
            int n = cmd.ExecuteNonQuery();

            return $"【{key}】 成功刪除";
        }

        [HttpPut("{json}", Name = "PutWeatherForecastMySQL")]
        public string Put(string json)
        {
            //將 WeatherForecast 資料表中修改 Summaries 欄位值為指定內容
            string sql = $"UPDATE WeatherForecast SET Summaries='{json}' WHERE id='1'";
            using var connection = new MySqlConnection(_builder.ConnectionString);
            connection.Open();
            MySqlCommand cmd = new(sql, connection);
            int n = cmd.ExecuteNonQuery();

            return $"【{json}】 更新 {n} 筆資料成功";
        }
    }
}