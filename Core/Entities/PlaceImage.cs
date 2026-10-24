namespace Domain_Layer.Entities;

public class PlaceImage
{
    // ملاحظة تم تغيرة
    public Guid PlaceImageId { get; init; }
    public string ImageUrl { get; set; } = null!;
    //PlaceDetails Relationship M: 1
    public int PlaceId { get; set; }
    public Place Place { get; set; } = null!;
}
