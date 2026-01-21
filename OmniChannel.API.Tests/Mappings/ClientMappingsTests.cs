using FluentAssertions;
using OmniChannel.API.DTOs;
using OmniChannel.API.Mappings;
using OmniChannel.API.Models;

namespace OmniChannel.API.Tests.Mappings
{
    [TestFixture]
    public class ClientMappingsTests
    {
        // -----------------------
        // ToDto
        // -----------------------
        [Test]
        public void ToDto_Should_Map_Client_To_ClientDto()
        {
            var createdAt = DateTime.UtcNow;

            var client = new Client
            {
                ClientID = 1,
                FullName = "John Doe",
                Email = "john@test.com",
                Status = "Active",
                AssignedManagerEmail = "manager@test.com",
                LastModifiedBy = "admin",
                CreatedAt = createdAt
            };

            var dto = client.ToDto();

            dto.ClientID.Should().Be(1);
            dto.FullName.Should().Be("John Doe");
            dto.Email.Should().Be("john@test.com");
            dto.Status.Should().Be("Active");
            dto.AssignedManagerEmail.Should().Be("manager@test.com");
            dto.LastModifiedBy.Should().Be("admin");
            dto.CreatedAt.Should().Be(createdAt);
        }

        // -----------------------
        // ToEntity (Create)
        // -----------------------
        [Test]
        public void ToEntity_Should_Map_CreateClientDto_And_Set_Defaults()
        {
            var dto = new CreateClientDto
            {
                FullName = "Jane Doe",
                Email = "jane@test.com",
                AssignedManagerEmail = null,
                Status = null,
                LastModifiedBy = null
            };

            var before = DateTime.UtcNow;

            var client = dto.ToEntity();

            client.FullName.Should().Be("Jane Doe");
            client.Email.Should().Be("jane@test.com");
            client.AssignedManagerEmail.Should().Be(string.Empty);
            client.Status.Should().Be("Active");
            client.LastModifiedBy.Should().Be("CoreAPI");
            client.CreatedAt.Should().BeOnOrAfter(before);
        }

        [Test]
        public void ToEntity_Should_Use_Status_When_Provided()
        {
            var dto = new CreateClientDto
            {
                FullName = "Jane Doe",
                Email = "jane@test.com",
                Status = "Inactive"
            };

            var client = dto.ToEntity();

            client.Status.Should().Be("Inactive");
        }

        // -----------------------
        // ApplyUpdates
        // -----------------------
        [Test]
        public void ApplyUpdates_Should_Update_All_Updatable_Fields()
        {
            var client = new Client
            {
                FullName = "Old Name",
                Email = "old@test.com",
                AssignedManagerEmail = "old.manager@test.com",
                Status = "Active",
                LastModifiedBy = "admin"
            };

            var dto = new UpdateClientDto
            {
                FullName = "New Name",
                Email = "new@test.com",
                AssignedManagerEmail = "new.manager@test.com",
                Status = "Inactive",
                LastModifiedBy = "editor"
            };

            client.ApplyUpdates(dto);

            client.FullName.Should().Be("New Name");
            client.Email.Should().Be("new@test.com");
            client.AssignedManagerEmail.Should().Be("new.manager@test.com");
            client.Status.Should().Be("Inactive");
            client.LastModifiedBy.Should().Be("editor");
        }

        [Test]
        public void ApplyUpdates_Should_Not_Overwrite_With_Null_Values()
        {
            var client = new Client
            {
                FullName = "Existing Name",
                Email = "existing@test.com",
                AssignedManagerEmail = "manager@test.com",
                Status = "Active",
                LastModifiedBy = "admin"
            };

            var dto = new UpdateClientDto
            {
                FullName = "Updated Name",
                Email = "updated@test.com",
                AssignedManagerEmail = null,
                LastModifiedBy = null,
                Status = "Inactive"
            };

            client.ApplyUpdates(dto);

            client.AssignedManagerEmail.Should().Be("manager@test.com");
            client.LastModifiedBy.Should().Be("admin");
        }
    }
}
