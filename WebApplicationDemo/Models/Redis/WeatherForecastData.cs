namespace WebApplicationDemo.Models.Cache
{
    /// <summary>
    /// 天氣預報資訊
    /// </summary>
    public class WeatherForecastData
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
