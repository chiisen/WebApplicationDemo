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
        private Mock<ILogger<WeatherForecastRedisController>> _mockLogger = null!;
        private Mock<ICacheService> _mockCache = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<WeatherForecastRedisController>>();
            _mockCache = new Mock<ICacheService>();
        }

        [TestMethod]
        public void Get_WhenCacheReturnsValidJson_ReturnsWeatherForecast()
        {
            var jsonData = "[\"Sunny\",\"Cloudy\",\"Rainy\"]";
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, jsonData));

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var results = controller.Get();

            results.Should().NotBeNull();
            results.Code.Should().Be(0);
            results.Data.Should().NotBeNull();
            results.Data.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Get_WhenCacheReadFails_ReturnsFail()
        {
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(false, string.Empty));

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var results = controller.Get();

            results.Should().NotBeNull();
            results.Code.Should().Be((int)ResponseCode.Fail);
        }

        [TestMethod]
        public void Get_WhenCacheReturnsInvalidJson_ReturnsFail()
        {
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, "InvalidJson"));

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var results = controller.Get();

            results.Should().NotBeNull();
            results.Code.Should().Be((int)ResponseCode.Fail);
        }

        [TestMethod]
        public void Post_WhenCacheSetSuccess_ReturnsSuccess()
        {
            _mockCache.Setup(x => x.SetCacheKey(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(true);

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Post("test.json");

            result.Should().Contain("新增成功");
        }

        [TestMethod]
        public void Post_WhenCacheSetFails_ReturnsFail()
        {
            _mockCache.Setup(x => x.SetCacheKey(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(false);

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Post("test.json");

            result.Should().Contain("新增失敗");
        }

        [TestMethod]
        public void Delete_WhenKeyExists_ReturnsSuccess()
        {
            _mockCache.Setup(x => x.KeyDelete(It.IsAny<string>()))
                     .Returns(true);

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Delete("testKey");

            result.Should().Contain("成功刪除");
        }

        [TestMethod]
        public void Delete_WhenKeyNotExists_ReturnsFail()
        {
            _mockCache.Setup(x => x.KeyDelete(It.IsAny<string>()))
                     .Returns(false);

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Delete("testKey");

            result.Should().Contain("刪除失敗");
        }

        [TestMethod]
        public void Put_WhenKeyExistsAndUpdateSuccess_ReturnsSuccess()
        {
            var jsonData = "[\"Sunny\",\"Cloudy\",\"Rainy\"]";
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, jsonData));
            _mockCache.Setup(x => x.SetCacheKey(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(true);

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Put("Sunny", "Windy");

            result.Should().Contain("成功");
        }

        [TestMethod]
        public void Put_WhenKeyNotFound_ReturnsFail()
        {
            var jsonData = "[\"Sunny\",\"Cloudy\",\"Rainy\"]";
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, jsonData));

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Put("NonExistent", "Windy");

            result.Should().Contain("找不到");
        }

        [TestMethod]
        public void Put_WhenCacheReadFails_ReturnsFail()
        {
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(false, string.Empty));

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Put("Sunny", "Windy");

            result.Should().Contain("失敗");
        }

        [TestMethod]
        public void Put_WhenCacheUpdateFails_ReturnsFail()
        {
            var jsonData = "[\"Sunny\",\"Cloudy\",\"Rainy\"]";
            _mockCache.Setup(x => x.GetCacheKey(It.IsAny<string>()))
                     .Returns(new Tuple<bool, string>(true, jsonData));
            _mockCache.Setup(x => x.SetCacheKey(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(false);

            var controller = new WeatherForecastRedisController(_mockLogger.Object, _mockCache.Object);
            var result = controller.Put("Sunny", "Windy");

            result.Should().Contain("失敗");
        }
    }
}
