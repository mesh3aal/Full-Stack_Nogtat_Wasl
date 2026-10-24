
namespace Application_Layer.Models.ReciveDTOs.DashboardDTOs.AdminDashboardController
{
    public class RequestListResponse
    {
        public List<LastOrderDto> LastOrders { get; set; } = new List<LastOrderDto>();
    }
}
