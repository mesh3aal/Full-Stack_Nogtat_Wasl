namespace Application_Layer.Models.ResponseDTO.PlaceController;

public record ListPlaces<T>
{
    public List<T> Places { get; set; } = new List<T>();
    public int RemainingPlacesCount { get; set; }
}