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

        private readonly string _key = "Summaries";

        public WeatherForecastMS_SQLController(ILogger<WeatherForecastMS_SQLController> logger)
        {
            _logger = logger;

            _logger.LogInformation("初始化 WeatherForecastMs-SQL");
        }

        [HttpGet(Name = "GetWeatherForecastMsSQL")]
        public IEnumerable<WeatherForecastMS_SQL> Get()
        {
            using SqlConnection connection = new(ConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();
            command.CommandText = @"SELECT * FROM WeatherForecast;";
            /*
            command.Parameters.AddWithValue("@name", name);
            */

            List<string> Summaries_ = new();
            using (connection)
            {
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Summaries_.Add(Convert.ToString(reader[_key]));
                }
            }

            command.Connection.Close();


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
            //在 WeatherForecast 資料表新增一筆資料
            using SqlConnection connection = new SqlConnection(ConnectionString);

            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();

            // 取得新增資料後自動產生的 id
            command.CommandText = $"INSERT INTO WeatherForecast ( Summaries ) VALUES ( '{weather}')";
            int id = Convert.ToInt32(command.ExecuteScalar());
            command.Connection.Close();

            return $"【{weather}】 新增 {id} 筆資料成功";
        }

        /// <summary>
        /// Redis 刪除一筆資料
        /// </summary>
        /// <param name="key">指定要刪除的 Redis Key</param>
        /// <returns>回傳執行結果</returns>
        [HttpDelete("{key}", Name = "DeleteWeatherForecastMsSQL")]
        public string Delete(string key)
        {
            //刪除 WeatherForecast 資料表中 Summaries 欄位值為指定的資料
            using SqlConnection connection = new SqlConnection(ConnectionString);

            string sql = $"DELETE FROM WeatherForecast WHERE Summaries='{key}'";

            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();
            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Connection.Close();

            return $"【{key}】 成功刪除";
        }

        [HttpPut("{weather}", Name = "PutWeatherForecastMsSQL")]
        public string Put(string weather, string weatherNew)
        {
            //將 WeatherForecast 資料表中修改 Summaries 欄位值為指定內容
            using SqlConnection connection = new SqlConnection(ConnectionString);

            string updateData = $"UPDATE WeatherForecast SET Summaries='{weatherNew}' WHERE Summaries='{weather}'";

            using SqlCommand command = connection.CreateCommand();
            command.Connection.Open();
            command.CommandText = updateData;
            command.ExecuteNonQuery();
            command.Connection.Close();

            return $"【{weather}】 更新為 【{weatherNew}】 成功";
        }
    }
}