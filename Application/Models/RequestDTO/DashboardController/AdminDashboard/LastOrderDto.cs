
namespace Application_Layer.Models.ReciveDTOs.DashboardDTOs.AdminDashboardController
{
    public class LastOrderDto
    {
        public int Id { get; set; } 
        public string NamePlace { get; set; } =string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public RequestStates State { get; set; }
       


    }
}
