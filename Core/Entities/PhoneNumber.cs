namespace Domain_Layer.Entities;

public class PhoneNumber
{
    public string Number { get; set; } = null!;
    public int PlaceId { get; set; }
    public Place? Place { get; set; }
}
