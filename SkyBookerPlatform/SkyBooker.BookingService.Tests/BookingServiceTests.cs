using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SkyBooker.BookingService.Dtos;
using SkyBooker.BookingService.Entities;
using SkyBooker.BookingService.Interfaces;
using SkyBooker.BookingService.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SkyBooker.BookingService.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        private Mock<IBookingRepository> _bookingRepoMock;
        private Mock<ILogger<BookingServiceImpl>> _loggerMock;
        private BookingServiceImpl _bookingService;

        [TestInitialize]
        public void Setup()
        {
            _bookingRepoMock = new Mock<IBookingRepository>();
            _loggerMock = new Mock<ILogger<BookingServiceImpl>>();
            _bookingService = new BookingServiceImpl(_bookingRepoMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task CreateBookingAsync_ShouldReturnSuccess_WithValidData()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                FlightId = 1,
                SeatNumber = "1A",
                TotalAmount = 5000,
                PassengerName = "Test Passenger",
                PassengerEmail = "test@gmail.com"
            };

            _bookingRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<Booking>()))
                            .ReturnsAsync(new Booking { Id = 1 });

            // Act
            var result = await _bookingService.CreateBookingAsync(2, dto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("Confirmed", result.Data.Status);
            _bookingRepoMock.Verify(repo => repo.CreateAsync(It.IsAny<Booking>()), Times.Once);
        }
    }
}
