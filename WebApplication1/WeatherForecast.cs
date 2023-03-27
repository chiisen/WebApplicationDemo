namespace WebApplication1
{
    public class WeatherForecast
    {
        /// <summary>
        /// Date of the weather
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Date of the weather
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Summary of the Weather. It can be "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        /// </summary>
        public string? Summary { get; set; }
    }
}