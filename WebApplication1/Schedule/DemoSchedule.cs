using Coravel.Invocable;

namespace WebApplication1.Schedule
{
    public class DemoSchedule : IInvocable
    {
        private readonly ILogger<DemoSchedule> _logger;
        public DemoSchedule(ILogger<DemoSchedule> logger)
        {
            _logger = logger;
        }
        public async Task Invoke()
        {
            _logger.LogInformation("Invoke DemoSchedule on time : {time}", DateTime.Now);
            _logger.LogInformation("DemoSchedule Done");
            await Task.CompletedTask;
        }
    }
}
