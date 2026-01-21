using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OmniChannel.API.Controllers;
using OmniChannel.API.DTOs;
using OmniChannel.API.Models;
using OmniChannel.API.Repositories;

namespace OmniChannel.API.Tests.Controllers
{
    [TestFixture]
    public class ClientControllerTests
    {
        private Mock<IClientRepository> _repoMock = null!;
        private ClientController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IClientRepository>();
            _controller = new ClientController(_repoMock.Object);
        }

        // -------------------------
        // GET: api/clients
        // -------------------------
        [Test]
        public async Task GetClients_Should_Return_Ok_With_ClientDtos()
        {
            var clients = new List<Client>
            {
                new Client { ClientID = 1, FullName = "John", Email = "john@test.com" },
                new Client { ClientID = 2, FullName = "Jane", Email = "jane@test.com" }
            };

            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(clients);

            var result = await _controller.GetClients();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var dtoList = okResult!.Value as IEnumerable<ClientDto>;
            dtoList.Should().HaveCount(2);
        }

        // -------------------------
        // GET: api/clients/{id}
        // -------------------------
        [Test]
        public async Task GetClient_Should_Return_Ok_When_Client_Exists()
        {
            var client = new Client
            {
                ClientID = 1,
                FullName = "John",
                Email = "john@test.com"
            };

            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(client);

            var result = await _controller.GetClient(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var dto = okResult!.Value as ClientDto;
            dto!.ClientID.Should().Be(1);
        }

        [Test]
        public async Task GetClient_Should_Return_NotFound_When_Client_Does_Not_Exist()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync((Client?)null);

            var result = await _controller.GetClient(1);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        // -------------------------
        // POST: api/clients
        // -------------------------
        [Test]
        public async Task CreateClient_Should_Return_CreatedAtAction()
        {
            var dto = new CreateClientDto
            {
                FullName = "New Client",
                Email = "new@test.com"
            };

            var createdClient = new Client
            {
                ClientID = 10,
                FullName = dto.FullName,
                Email = dto.Email
            };

            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Client>()))
                     .ReturnsAsync(createdClient);

            var result = await _controller.CreateClient(dto);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.RouteValues!["id"].Should().Be(10);
        }

        [Test]
        public async Task CreateClient_Should_Return_BadRequest_When_ModelState_Invalid()
        {
            _controller.ModelState.AddModelError("FullName", "Required");

            var dto = new CreateClientDto();

            var result = await _controller.CreateClient(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // -------------------------
        // PUT: api/clients/{id}
        // -------------------------
        [Test]
        public async Task UpdateClient_Should_Return_NoContent_When_Successful()
        {
            var client = new Client
            {
                ClientID = 1,
                FullName = "Old Name",
                Email = "old@test.com"
            };

            _repoMock.Setup(r => r.ExistsAsync(1))
                     .ReturnsAsync(true);

            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(client);

            var dto = new UpdateClientDto
            {
                FullName = "Updated Name",
                Status = "Inactive"
            };

            var result = await _controller.UpdateClient(1, dto);

            result.Should().BeOfType<NoContentResult>();
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task UpdateClient_Should_Return_NotFound_When_Client_Does_Not_Exist()
        {
            _repoMock.Setup(r => r.ExistsAsync(1))
                     .ReturnsAsync(false);

            var dto = new UpdateClientDto();

            var result = await _controller.UpdateClient(1, dto);

            result.Should().BeOfType<NotFoundResult>();
        }

        // -------------------------
        // DELETE: api/clients/{id}
        // -------------------------
        [Test]
        public async Task DeleteClient_Should_Return_NoContent_When_Successful()
        {
            _repoMock.Setup(r => r.ExistsAsync(1))
                     .ReturnsAsync(true);

            var result = await _controller.DeleteClient(1);

            result.Should().BeOfType<NoContentResult>();
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Test]
        public async Task DeleteClient_Should_Return_NotFound_When_Client_Does_Not_Exist()
        {
            _repoMock.Setup(r => r.ExistsAsync(1))
                     .ReturnsAsync(false);

            var result = await _controller.DeleteClient(1);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
