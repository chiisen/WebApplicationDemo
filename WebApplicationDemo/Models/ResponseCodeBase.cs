namespace WebApplicationDemo.Models
{
    public class ResponseCodeBase
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ResponseCodeBase()
        {
            Code = (int)ResponseCode.Success;
            Message = ResponseMessageCode.Message[Code];
        }
    };
}
