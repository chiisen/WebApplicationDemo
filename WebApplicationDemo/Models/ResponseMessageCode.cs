namespace WebApplicationDemo.Models
{
    public class ResponseMessageCode
    {
        public static Dictionary<int, string> Message = new Dictionary<int, string>()
        {
            { (int)ResponseCode.Success, "Success" },
            { (int)ResponseCode.Fail, "Fail" }
        };
    }
}
