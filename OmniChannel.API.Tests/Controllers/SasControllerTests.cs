using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OmniChannel.API.Controllers;
using OmniChannel.API.Repositories;

namespace OmniChannel.API.Tests.Controllers
{
    [TestFixture]
    public class SasControllerTests
    {
        private Mock<IActivityUploadRepository> _repositoryMock;
        private SasController _controller;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IActivityUploadRepository>();
            _controller = new SasController(_repositoryMock.Object);
        }

        [Test]
        public void GetUploadLink_ReturnsBadRequest_WhenFilenameIsNull()
        {
            // Act
            var result = _controller.GetUploadLinkAsync(null);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public void GetUploadLink_ReturnsBadRequest_WhenFilenameIsEmpty()
        {
            // Act
            var result = _controller.GetUploadLinkAsync("");

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value, Is.EqualTo("Filename is required"));
        }

        [Test]
        public void GetUploadLink_ReturnsOk_WhenFilenameIsValid()
        {
            // Arrange
            const string sasUrl = "https://blob.core.windows.net/container/file.json?sas";

            _repositoryMock
                .Setup(r => r.GenerateUploadSasUrl(It.IsAny<string>()))
                .Returns(sasUrl);

            // Act
            var result = _controller.GetUploadLinkAsync("file.json");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var value = okResult!.Value!;
            var uploadUrlProp = value.GetType().GetProperty("uploadUrl")?.GetValue(value);

            Assert.That(uploadUrlProp, Is.EqualTo(sasUrl));
        }
    }
}