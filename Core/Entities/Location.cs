namespace Domain_Layer.Entities;

public class Location
{
    public int Id { get; set; }
    public string NearestLandmark { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int DirectorateId { get; set; }
    public int DistrictId { get; set; }
    public int PlaceId { get; set; }
    public Directorate Directorate { get; set; } = null!;
    public District District { get; set; } = null!;
}
