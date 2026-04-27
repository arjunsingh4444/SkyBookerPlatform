using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SkyBooker.AuthService.Dtos;
using SkyBooker.AuthService.Entities;
using SkyBooker.AuthService.Interfaces;
using SkyBooker.AuthService.Services;
using System.Threading.Tasks;

namespace SkyBooker.AuthService.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<Microsoft.Extensions.Logging.ILogger<AuthServiceImpl>> _loggerMock;
        private AuthServiceImpl _authService;

        [TestInitialize]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<AuthServiceImpl>>();
            _authService = new AuthServiceImpl(_userRepoMock.Object, _tokenServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenEmailIsUnique()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            _userRepoMock.Setup(repo => repo.ExistsByEmailAsync(registerDto.Email))
                         .ReturnsAsync(false);

            _userRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                         .ReturnsAsync(new User { Id = 1, Email = registerDto.Email });

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("User registered successfully", result.Message);
            _userRepoMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldFail_WhenEmailAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterDto { Email = "test@example.com", Password = "Password123" };
            _userRepoMock.Setup(repo => repo.ExistsByEmailAsync(registerDto.Email))
                         .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("A user with this email already exists", result.Message);
            _userRepoMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
