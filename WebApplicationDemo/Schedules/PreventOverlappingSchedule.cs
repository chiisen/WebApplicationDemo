using Coravel.Invocable;

namespace WebApplicationDemo.Schedules
{
    /// <summary>
    /// 另一個實例仍在運行，Coravel 將忽略當前到期的任務
    /// </summary>
    public class PreventOverlappingSchedule : IInvocable
    {
        private readonly ILogger<PreventOverlappingSchedule> _logger;

        private static bool _firstRun = true;

        private string _guid;

        public PreventOverlappingSchedule(ILogger<PreventOverlappingSchedule> logger)
        {
            _logger = logger;

            int len_ = 12;//指定 Guid 的長度
            _guid = Guid.NewGuid().ToString().Substring(0, len_);
        }
        public async Task Invoke()
        {
            if (_firstRun)
            {
                _logger.LogInformation($"PreventOverlappingSchedule【{_guid}】 💠 First Run");
                _firstRun = false;
            }

            // Wait for 5 seconds
            await Task.Delay(15000);

            _logger.LogInformation($"Invoke PreventOverlappingSchedule【{_guid}】 on time : {DateTime.Now}");
            _logger.LogInformation($"PreventOverlappingSchedule【{_guid}】 Done");
            await Task.CompletedTask;
        }
    }
}
