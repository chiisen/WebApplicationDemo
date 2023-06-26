using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplicationDemo.Controllers;
using WebApplicationDemo.Models;
using WebApplicationDemo.Services.Common;


namespace MSTest
{
    [TestClass]
    public class UnitTest_Controller
    {
        [TestMethod]
        public void TestMethod_Controller()
        {
            //透過mock將外界的介面包起來
            var MockLogger = new Mock<ILogger<WeatherForecastRedisController>>();

            var MockCache = new Mock<ICacheService>();
            MockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, "MSTest"));
            MockCache.Setup(x => x.SetCacheKey(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(true);
            MockCache.Setup(x => x.KeyDelete(It.IsAny<string>()))
                     .Returns(true);
            //當成物件傳入controller，代替實際的介面
            var Controllers = new WeatherForecastRedisController(MockLogger.Object, MockCache.Object);

            //執行要測試的函式
            var Results = Controllers.Get();

            //確認結果不為null
            Results.Should().NotBeNull();

            Results.Code.Should().Be((int)ResponseCode.Fail);
        }
    }
}
