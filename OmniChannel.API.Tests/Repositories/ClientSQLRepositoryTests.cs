using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OmniChannel.API.Data;
using OmniChannel.API.Models;
using OmniChannel.API.Repositories;
using NUnit.Framework;

namespace OmniChannel.API.Tests.Repositories
{
    [TestFixture]
    public class ClientSQLRepositoryTests
    {
        private AppDbContext _context = null!;
        private ClientSQLRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ClientSQLRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // ------------------------
        // GetAllAsync
        // ------------------------
        [Test]
        public async Task GetAllAsync_Should_Return_All_Clients()
        {
            _context.Clients.AddRange(
                new Client
                {
                    FullName = "Client One",
                    Email = "client1@test.com"
                },
                new Client
                {
                    FullName = "Client Two",
                    Email = "client2@test.com"
                }
            );

            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            result.Should().HaveCount(2);
        }

        // ------------------------
        // GetByIdAsync
        // ------------------------
        [Test]
        public async Task GetByIdAsync_Should_Return_Client_When_Exists()
        {
            var client = new Client
            {
                FullName = "John Doe",
                Email = "john@test.com",
                Status = "Active"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(client.ClientID);

            result.Should().NotBeNull();
            result!.FullName.Should().Be("John Doe");
            result.Email.Should().Be("john@test.com");
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var result = await _repository.GetByIdAsync(999);

            result.Should().BeNull();
        }

        // ------------------------
        // CreateAsync
        // ------------------------
        [Test]
        public async Task CreateAsync_Should_Add_Client_And_Set_Defaults()
        {
            var client = new Client
            {
                FullName = "New Client",
                Email = "new@test.com"
            };

            var result = await _repository.CreateAsync(client);

            result.ClientID.Should().BeGreaterThan(0);
            result.Status.Should().Be("Active");
            result.LastModifiedBy.Should().Be("CoreAPI");
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        // ------------------------
        // UpdateAsync
        // ------------------------
        [Test]
        public async Task UpdateAsync_Should_Update_Client()
        {
            var client = new Client
            {
                FullName = "Old Name",
                Email = "old@test.com"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            client.FullName = "Updated Name";
            client.Status = "Inactive";

            await _repository.UpdateAsync(client);

            var updated = await _context.Clients.FirstAsync();
            updated.FullName.Should().Be("Updated Name");
            updated.Status.Should().Be("Inactive");
        }

        // ------------------------
        // DeleteAsync
        // ------------------------
        [Test]
        public async Task DeleteAsync_Should_Remove_Client_When_Exists()
        {
            var client = new Client
            {
                FullName = "Delete Me",
                Email = "delete@test.com"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(client.ClientID);

            _context.Clients.Should().BeEmpty();
        }

        [Test]
        public async Task DeleteAsync_Should_Do_Nothing_When_Client_Not_Found()
        {
            await _repository.DeleteAsync(123);

            _context.Clients.Should().BeEmpty();
        }

        // ------------------------
        // ExistsAsync
        // ------------------------
        [Test]
        public async Task ExistsAsync_Should_Return_True_When_Client_Exists()
        {
            var client = new Client
            {
                FullName = "Exists Client",
                Email = "exists@test.com"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsAsync(client.ClientID);

            exists.Should().BeTrue();
        }

        [Test]
        public async Task ExistsAsync_Should_Return_False_When_Client_Does_Not_Exist()
        {
            var exists = await _repository.ExistsAsync(999);

            exists.Should().BeFalse();
        }
    }
}
