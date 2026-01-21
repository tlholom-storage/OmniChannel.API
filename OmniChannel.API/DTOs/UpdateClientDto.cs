using System.ComponentModel.DataAnnotations;

namespace OmniChannel.API.DTOs
{
    public class UpdateClientDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? AssignedManagerEmail { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? LastModifiedBy { get; set; }
    }
}
