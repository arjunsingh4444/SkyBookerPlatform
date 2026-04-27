using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SkyBooker.PassengerService.Dtos;
using SkyBooker.PassengerService.Entities;
using SkyBooker.PassengerService.Interfaces;
using SkyBooker.PassengerService.Services;
using System;
using System.Threading.Tasks;

namespace SkyBooker.PassengerService.Tests
{
    [TestClass]
    public class PassengerServiceTests
    {
        private Mock<IPassengerRepository> _repoMock;
        private PassengerServiceImpl _service;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IPassengerRepository>();
            _service = new PassengerServiceImpl(_repoMock.Object);
        }

        [TestMethod]
        public async Task CreatePassengerAsync_ShouldSucceed()
        {
            // Arrange
            var dto = new PassengerDto
            {
                FirstName = "Arjun",
                LastName = "Singh",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Nationality = "Indian"
            };

            _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Passenger>()))
                     .ReturnsAsync(new Passenger { Id = 1 });

            // Act
            var result = await _service.CreatePassengerAsync(2, dto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Arjun", result.Data.FirstName);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Passenger>()), Times.Once);
        }
    }
}
