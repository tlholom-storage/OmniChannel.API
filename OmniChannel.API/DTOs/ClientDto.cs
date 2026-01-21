namespace OmniChannel.API.DTOs
{
    public class ClientDto
    {
        public int ClientID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AssignedManagerEmail { get; set; } = string.Empty;
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
