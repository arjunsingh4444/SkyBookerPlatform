using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SkyBooker.SeatService.Dtos;
using SkyBooker.SeatService.Entities;
using SkyBooker.SeatService.Interfaces;
using SkyBooker.SeatService.Services;
using System.Threading.Tasks;

namespace SkyBooker.SeatService.Tests
{
    [TestClass]
    public class SeatServiceTests
    {
        private Mock<ISeatRepository> _seatRepoMock;
        private Mock<Microsoft.Extensions.Logging.ILogger<SeatServiceImpl>> _loggerMock;
        private SeatServiceImpl _seatService;

        [TestInitialize]
        public void Setup()
        {
            _seatRepoMock = new Mock<ISeatRepository>();
            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<SeatServiceImpl>>();
            _seatService = new SeatServiceImpl(_seatRepoMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task LockSeatAsync_ShouldReturnSuccess_WhenSeatIsAvailable()
        {
            // Arrange
            var dto = new SeatActionDto { FlightId = 1, SeatNumber = "1A", UserId = 2 };

            var seat = new Seat { Id = 10, FlightId = dto.FlightId, SeatNumber = dto.SeatNumber, Status = SeatStatus.Available };
            
            _seatRepoMock.Setup(repo => repo.GetByFlightAndSeatNumberAsync(dto.FlightId, dto.SeatNumber))
                         .ReturnsAsync(seat);
            
            // Act
            var result = await _seatService.LockSeatAsync(dto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(SeatStatus.Locked, seat.Status);
            Assert.AreEqual(dto.UserId, seat.LockedByUserId);
            _seatRepoMock.Verify(repo => repo.UpdateAsync(seat), Times.Once);
        }
        
        [TestMethod]
        public async Task LockSeatAsync_ShouldFail_WhenSeatAlreadyBooked()
        {
            // Arrange
            var dto = new SeatActionDto { FlightId = 1, SeatNumber = "1A", UserId = 2 };
            var seat = new Seat { Id = 10, FlightId = 1, SeatNumber = "1A", Status = SeatStatus.Booked };
            
            _seatRepoMock.Setup(repo => repo.GetByFlightAndSeatNumberAsync(1, "1A"))
                         .ReturnsAsync(seat);
            
            // Act
            var result = await _seatService.LockSeatAsync(dto);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Seat is already booked", result.Message);
            _seatRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Seat>()), Times.Never);
        }
    }
}
