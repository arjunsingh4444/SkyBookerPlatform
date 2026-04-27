using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SkyBooker.NotificationService.Controllers;
using SkyBooker.NotificationService.Dtos;
using SkyBooker.NotificationService.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SkyBooker.NotificationService.Tests
{
    [TestClass]
    public class NotificationControllerTests
    {
        private Mock<ILogger<NotificationController>> _loggerMock;
        private NotificationDbContext _dbContext;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Use In-Memory DB for testing the controller directly
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestNotificationDb")
                .Options;

            _dbContext = new NotificationDbContext(options);
            _loggerMock = new Mock<ILogger<NotificationController>>();
            _controller = new NotificationController(_dbContext, _loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task SendNotification_ShouldReturnOk_AndLogToDb()
        {
            // Arrange
            var dto = new SendNotificationDto
            {
                Recipient = "test@gmail.com",
                Subject = "Hello",
                Message = "Test Message",
                Type = "Email"
            };

            // Act
            var result = await _controller.SendNotification(dto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            
            var logs = await _dbContext.NotificationLogs.ToListAsync();
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("test@gmail.com", logs[0].Recipient);
        }
    }
}
