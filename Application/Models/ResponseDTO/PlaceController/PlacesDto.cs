
using Application_Layer.Models.ResponseDTO.ReviewController;

namespace Application_Layer.Models.ResponseDTO.PlaceController;

public record PlaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string CategoryName { get; set; } = null!;
    public string SubCategoryName { get; set; } = null!;
    public PlaceStatus State { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    public List<WeekDays> UnavailableAt { get; set; } = [];
    public string? NearestLandMark { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string DistrictName { get; set; } = null!;
    public string DirectorateName { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = [];
    public List<string> Contacts { get; set; } = [];
    public string AverageRate { get; set; } = null!;
    public string RateCount { get; set; } = null!;
}
public record PlaceRatesDto
{
    public PlaceDto PlaceDetail { get; set; } = null!;
    public OneToFiveRating OneToFiveRating { get; set; } = null!;
    public ListReviews<ReviewResponse> Rates { get; set; } = null!;
}
public record UserRate
{
    public string UserName { get; set; } = null!;
    public string UserImage { get; set; } = null!;
    public sbyte Rate { get; set; } = 0; 
    public DateTime CreatedAt { get; set; }
    public string? Note { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
}

public record OneToFiveRating
{
    public int FiveStar { get; set; } = 0;
    public int FourStar { get; set; } = 0;
    public int ThreeStar { get; set; } = 0;
    public int TwoStar { get; set; } = 0;
    public int OneStar { get; set; } = 0;
}
