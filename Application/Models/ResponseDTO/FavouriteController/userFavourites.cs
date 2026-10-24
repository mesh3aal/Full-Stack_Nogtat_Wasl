
namespace Application_Layer.Models.ResponseDTO.FavouriteController;

public record UserFavourites
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; } = null!;
    public string mainCategoryName { get; set; } = null!;
    public string subcategoryName { get; set; } = null!;
    public string? Image { get; set; } = string.Empty;
    public string AverageRating { get; set; } = null!;
    public string directorateName { get; set; } = null!;
    public string districtName { get; set; } = null!;
    public DateTime FavouritedAt { get; set; }
}
