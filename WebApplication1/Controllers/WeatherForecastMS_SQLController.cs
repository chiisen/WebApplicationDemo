using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastMS_SQLController : ControllerBase
    {
        private readonly ILogger<WeatherForecastMS_SQLController> _logger;

        private const string ConnectionString =
        "Server=localhost;Database=dev;User=sa;Password=Pass@word;TrustServerCertificate=true";

        private readonly string _field = "Summaries";

        public WeatherForecastMS_SQLController(ILogger<WeatherForecastMS_SQLController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecastMsSQL")]
        public IEnumerable<WeatherForecastMS_SQL> Get()
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 GetWeatherForecastMsSQL");

            using SqlConnection conn_ = new(ConnectionString);
            using SqlCommand cmd_ = conn_.CreateCommand();
            cmd_.Connection.Open();
            cmd_.CommandText = @"SELECT * FROM WeatherForecast;";
            /*
            cmd_.Parameters.AddWithValue("@name", name);
            */

            List<string> Summaries_ = new();
            using (conn_)
            {
                using SqlDataReader reader_ = cmd_.ExecuteReader();
                while (reader_.Read())
                {
                    var oneField_ = reader_[_field];
                    if(oneField_ != null)
                    {
                        string sum_ = Convert.ToString(oneField_);
                        Summaries_.Add(sum_);

                        _logger.LogInformation($"{convertedUUID_} 🚥 🍳撈取 MS-SQL 資料為 {sum_}");
                    }                    
                }
            }

            cmd_.Connection.Close();

            _logger.LogInformation($"{convertedUUID_} 🚥 🍳撈取 MS-SQL 資料結束");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecastMS_SQL
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
        [HttpPost(Name = "PostWeatherForecastMsSQL")]
        public string Post(string weather)
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 PostWeatherForecastMsSQL");

            //在 WeatherForecast 資料表新增一筆資料
            using SqlConnection connection = new(ConnectionString);

            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();

            // 取得新增資料後自動產生的 id
            command.CommandText = $"INSERT INTO WeatherForecast ( Summaries ) VALUES ( '{weather}')";
            command.Connection.Close();

            _logger.LogInformation($"{convertedUUID_} 🚥 【{weather}】 🌱新增資料成功");

            return $"【{weather}】 新增資料成功";
        }

        /// <summary>
        /// Redis 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastMsSQL")]
        public string Delete(string key)
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 DeleteWeatherForecastMsSQL");

            //刪除 WeatherForecast 資料表中 Summaries 欄位值為指定的資料
            using SqlConnection connection = new(ConnectionString);

            string sql = $"DELETE FROM WeatherForecast WHERE Summaries='{key}'";

            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();
            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Connection.Close();

            _logger.LogInformation($"{convertedUUID_} 🚥 【{key}】 成功🔥刪除");

            return $"【{key}】 成功刪除";
        }

        [HttpPut("{weather}", Name = "PutWeatherForecastMsSQL")]
        public string Put(string weather, string weatherNew)
        {
            Guid myUUId_ = Guid.NewGuid();
            string convertedUUID_ = myUUId_.ToString();

            _logger.LogInformation($"{convertedUUID_} 🚥 收到 PutWeatherForecastMsSQL");

            //將 WeatherForecast 資料表中修改 Summaries 欄位值為指定內容
            using SqlConnection connection = new(ConnectionString);

            string updateData = $"UPDATE WeatherForecast SET Summaries='{weatherNew}' WHERE Summaries='{weather}'";

            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();
            command.CommandText = updateData;
            command.ExecuteNonQuery();
            command.Connection.Close();

            _logger.LogInformation($"{convertedUUID_} 🚥 【{weather}】 ⚙更新為 【{weatherNew}】 成功");

            return $"【{weather}】 更新為 【{weatherNew}】 成功";
        }
    }
}