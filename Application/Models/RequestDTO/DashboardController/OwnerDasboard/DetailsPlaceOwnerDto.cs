
namespace Application_Layer.Models.ReciveDTOs.DashboardController.OwnerDasboard
{
    public class DetailsPlaceOwnerDto
    {
        public int PlaceId { get; set; }

        public string PLaceName { get; set; }
        public string Status { get; set; }
        public List<string> Imges { get; set; }

        public RatingDto Rating { get; set; }

        public List<ReviewDto> LatestReviews { get; set; }
    }


    public class RatingDto
    {
        public double Average { get; set; }
        public int TotalReviews { get; set; }

        public RatingDistributionDto Distribution { get; set; }
    }

    public class RatingDistributionDto
    {
        public int Five { get; set; }
        public int Four { get; set; }
        public int Three { get; set; }
        public int Two { get; set; }
        public int One { get; set; }
    }

    public class ReviewDto
    {
        public string UserName { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }


}
