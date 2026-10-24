
namespace Application_Layer.Models.ReciveDTOs.DashboardController.OwnerDasboard
{
    public class OwnerDashbordFilter
    {
        public string? Search { get; set; }

        public PlaceStatus? Status { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
