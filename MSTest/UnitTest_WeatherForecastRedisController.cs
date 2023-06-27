using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using WebApplicationDemo.Controllers;
using WebApplicationDemo.Models;
using WebApplicationDemo.Services.Common;


namespace MSTest
{
    /// <summary>
    /// 單元測試 WeatherForecastRedisController
    /// </summary>
    [TestClass]
    public class UnitTest_WeatherForecastRedisController
    {
        [TestMethod]
        public void TestMethod_Get()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            //透過 Mock 將外界的介面包起來
            var MockLogger = new Mock<ILogger<WeatherForecastRedisController>>();

            var MockCache = new Mock<ICacheService>();
            MockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, "MSTest"));
            MockCache.Setup(x => x.SetCacheKey(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(true);
            MockCache.Setup(x => x.KeyDelete(It.IsAny<string>()))
                     .Returns(true);

            Trace.WriteLine("Mock 完畢!");

            //當成物件傳入 Controller，代替實際的介面
            var controller = new WeatherForecastRedisController(MockLogger.Object, MockCache.Object);

            //執行要測試的函式
            var results = controller.Get();

            Trace.WriteLine("測試 完畢!");

            //確認結果不為null
            results.Should().NotBeNull();

            results.Code.Should().Be((int)ResponseCode.Fail);

            Trace.WriteLine($"results.Code: {results.Code}");
        }
    }
}
