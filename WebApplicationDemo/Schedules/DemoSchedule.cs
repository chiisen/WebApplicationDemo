using Coravel.Invocable;

namespace WebApplicationDemo.Schedules
{
    /// <summary>
    /// 一般 Schedule 測試
    /// </summary>
    public class DemoSchedule : IInvocable
    {
        private readonly ILogger<DemoSchedule> _logger;

        private static bool _firstRun = true;

        private string _guid;

        public DemoSchedule(ILogger<DemoSchedule> logger)
        {
            _logger = logger;

            int len_ = 12;//指定 Guid 的長度
            _guid = Guid.NewGuid().ToString().Substring(0, len_);
        }
        public async Task Invoke()
        {
            if(_firstRun)
            {
                _logger.LogInformation($"DemoSchedule【{_guid}】 💠 First Run");
                _firstRun = false;
            }
            _logger.LogInformation($"Invoke DemoSchedule【{_guid}】 on time : {DateTime.Now}");
            _logger.LogInformation($"DemoSchedule【{_guid}】 Done");
            await Task.CompletedTask;
        }
    }
}
