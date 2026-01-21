using System.ComponentModel.DataAnnotations;

namespace OmniChannel.API.Models
{
    public class Client
    {
        [Key]
        public int ClientID { get; set; } //PK

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Active";

        [EmailAddress]
        public string AssignedManagerEmail { get; set; } = string.Empty;

        public string LastModifiedBy { get; set; } = "CoreAPI";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
