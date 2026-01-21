using OmniChannel.API.DTOs;
using OmniChannel.API.Models;

namespace OmniChannel.API.Mappings
{
    public static class ClientMappings
    {
        public static ClientDto ToDto(this Client c)
        {
            return new ClientDto
            {
                ClientID = c.ClientID,
                FullName = c.FullName,
                Email = c.Email,
                Status = c.Status,
                AssignedManagerEmail = c.AssignedManagerEmail,
                LastModifiedBy = c.LastModifiedBy,
                CreatedAt = c.CreatedAt
            };
        }

        public static Client ToEntity(this CreateClientDto dto)
        {
            return new Client
            {
                FullName = dto.FullName,
                Email = dto.Email,
                AssignedManagerEmail = dto.AssignedManagerEmail ?? string.Empty,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status,
                LastModifiedBy = dto.LastModifiedBy ?? "CoreAPI",
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void ApplyUpdates(this Client target, UpdateClientDto dto)
        {
            target.FullName = dto.FullName;
            target.Email = dto.Email;
            target.AssignedManagerEmail = dto.AssignedManagerEmail ?? target.AssignedManagerEmail;
            target.Status = dto.Status;
            target.LastModifiedBy = dto.LastModifiedBy ?? target.LastModifiedBy;
        }
    }
}
