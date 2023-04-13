namespace WebApplicationDemo.Models.AppSettings.SqlSettings
{
    public class ISqlSettings
    {
        public string? Name { get; set; } = "MS-SQL";
        public string? Server { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
    }
}
