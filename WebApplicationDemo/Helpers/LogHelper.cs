using Microsoft.Extensions.Logging;

namespace WebApplicationDemo.Helpers
{
    public interface ILogHelper
    {
        string GenerateTraceId();
        void LogRequest(ILogger logger, string traceId, string endpoint);
        void LogError(ILogger logger, string traceId, string message, Exception? ex = null);
    }

    public class LogHelper : ILogHelper
    {
        public string GenerateTraceId()
        {
            return Guid.NewGuid().ToString();
        }

        public void LogRequest(ILogger logger, string traceId, string endpoint)
        {
            logger.LogInformation("{TraceId} 🚥 收到 {Endpoint}", traceId, endpoint);
        }

        public void LogError(ILogger logger, string traceId, string message, Exception? ex = null)
        {
            if (ex != null)
            {
                var errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0)?.GetFileLineNumber();
                var errorFile = new System.Diagnostics.StackTrace(ex, true).GetFrame(0)?.GetFileName();
                logger.LogError(ex, "{TraceId} 🚥 {Message} Error: {ErrorFile}.{ErrorLine}", 
                    traceId, message, errorFile, errorLine);
            }
            else
            {
                logger.LogError("{TraceId} 🚥 {Message}", traceId, message);
            }
        }
    }
}
