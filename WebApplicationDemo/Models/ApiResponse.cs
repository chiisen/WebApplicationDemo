namespace WebApplicationDemo.Models
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Code = (int)ResponseCode.Success,
                Message = message,
                Data = data,
                TraceId = Guid.NewGuid().ToString()
            };
        }

        public static ApiResponse<T> Fail(string message, int code = (int)ResponseCode.Fail)
        {
            return new ApiResponse<T>
            {
                Code = code,
                Message = message,
                Data = default,
                TraceId = Guid.NewGuid().ToString()
            };
        }
    }
}
