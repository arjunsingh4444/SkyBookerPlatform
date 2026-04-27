using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SkyBooker.FlightService.Dtos;
using SkyBooker.FlightService.Entities;
using SkyBooker.FlightService.Interfaces;
using SkyBooker.FlightService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyBooker.FlightService.Tests
{
    [TestClass]
    public class FlightServiceTests
    {
        private Mock<IFlightRepository> _flightRepoMock;
        private Mock<Microsoft.Extensions.Logging.ILogger<FlightServiceImpl>> _loggerMock;
        private FlightServiceImpl _flightService;

        [TestInitialize]
        public void Setup()
        {
            _flightRepoMock = new Mock<IFlightRepository>();
            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<FlightServiceImpl>>();
            _flightService = new FlightServiceImpl(_flightRepoMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task SearchFlightsAsync_ShouldReturnFlights_WhenMatchFound()
        {
            // Arrange
            string source = "Delhi";
            string dest = "Mumbai";
            DateTime date = DateTime.Today.AddDays(2);

            var mockFlights = new List<Flight>
            {
                new Flight { Id = 1, Source = "Delhi", Destination = "Mumbai", DepartureTime = date.AddHours(10) }
            };

            _flightRepoMock.Setup(repo => repo.SearchAsync(source, dest, date))
                           .ReturnsAsync(mockFlights);

            // Act
            var result = await _flightService.SearchFlightsAsync(source, dest, date);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Count);
            Assert.AreEqual("Delhi", result.Data[0].Source);
            _flightRepoMock.Verify(repo => repo.SearchAsync(source, dest, date), Times.Once);
        }
    }
}
