namespace Application_Layer.Models.ReciveDTOs.UserManagementController.UserManagementDto
{
    public class UserManegeResponse
    {
        public string ID { get; set; } = null!;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Rating { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string? Avatar { get; set; }
    }
}
