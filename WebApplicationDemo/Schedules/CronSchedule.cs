using Coravel.Invocable;

namespace WebApplicationDemo.Schedules
{
    public class CronSchedule : IInvocable
    {
        private readonly ILogger<CronSchedule> _logger;

        private static bool _firstRun = true;

        private string _guid;

        public CronSchedule(ILogger<CronSchedule> logger)
        {
            _logger = logger;

            int len_ = 12;//指定 Guid 的長度
            _guid = Guid.NewGuid().ToString().Substring(0, len_);
        }
        public async Task Invoke()
        {
            if (_firstRun)
            {
                _logger.LogInformation($"CronSchedule【{_guid}】 💠 First Run");
                _firstRun = false;
            }
            _logger.LogInformation($"Invoke CronSchedule【{_guid}】 on time : {DateTime.Now}");
            _logger.LogInformation($"CronSchedule【{_guid}】 Done");
            await Task.CompletedTask;
        }
    }
}
