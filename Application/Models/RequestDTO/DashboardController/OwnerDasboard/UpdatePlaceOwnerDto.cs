
namespace Application_Layer.Models.ReciveDTOs.DashboardController.OwnerDasboard
{
    public class UpdatePlaceOwnerDto
    {

        // Basic Info
        public string PlaceName { get; set; } = null!;
        public string CommercialRegisterNumber { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        // Location

        public string DirectorateName { get; set; } = null!;
        //// Location
        public string NearestLandmark { get; set; } = null!;
        // ÎØ ÇáÚÑÖ
        public double Latitude { get; set; }
        // ÎØ Øæá

        public double Longitude { get; set; }


        public int DirectorateId { get; set; }
        public int DistrictId { get; set; }


        // Categories
        public int MainCategoryId { get; set; }
        public int SubCategoryId { get; set; }

        // Details
        public string? Description { get; set; }

       
        public List<DayOfWeek>? WorkingDays { get; set; }

        // Contact
        public List<string> PhoneNumber { get; set; } = null!;


    }
}
