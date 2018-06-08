using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
{
    public interface IRepo
    {
        Task<List<string>> GetAllAsync();
    }

    public class DemoController
    {
        private IRepo _repo;

        public DemoController(IRepo repo)
        {
            _repo = repo;
        }

        public async Task<List<string>> GetData()
        {
            return await _repo.GetAllAsync();
        }
    }

    public class UnitTest1
    {
        private ILogger _logger;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TestOutput(testOutputHelper)
                .CreateLogger();
        }

        [Fact]
        public void PassedTest()
        {
            Assert.True(1 + 1 == 2);
        }

        [Fact]
        public void FailedTest()
        {
            _logger.Information("This test will fail.");
            Assert.True(false);
        }

        [Fact]
        public async Task DemoMoq()
        {
            //Arrange
            var mockRepo = new Mock<IRepo>();
            mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(GetMockedData()));

            var controller = new DemoController(mockRepo.Object);

            //Act
            var result = await controller.GetData();

            //Assert
            //Compare one by one
            Assert.Collection(result, item => Assert.Equal("item0", item), item => Assert.Equal("item1", item),
                item => Assert.Equal("item2", item));
            
            //Compare at once
            Assert.Equal(new[] {"item0", "item1", "item2"}.ToList(), result);
        }

        private List<string> GetMockedData()
        {
            return new[] {"item0", "item1", "item2"}.ToList();
        }
    }
}