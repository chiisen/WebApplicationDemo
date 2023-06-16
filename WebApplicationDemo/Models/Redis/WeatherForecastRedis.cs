using WebApplicationDemo.Models.Cache;

namespace WebApplicationDemo.Models.Redis
{
    public class WeatherForecastRedis : ResponseCodeBase
    {
        /// <summary>
        /// 一周的天氣預報
        /// </summary>
        public WeatherForecastData[]? Data;
    }
}
