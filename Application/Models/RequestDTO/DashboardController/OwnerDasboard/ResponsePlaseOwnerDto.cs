
namespace Application_Layer.Models.ReciveDTOs.DashboardController.OwnerDasboard
{
    public class ResponsePlaseOwnerDto
    {
        public int id { get; set; }
        public string PlaceName { get; set; }
        public PlaceStatus Status { get; set; }
        public double AverageRate { get; set; }
        public int RatesCount { get; set; }
        public string? imgeUrl { get; set; }




    }
}
