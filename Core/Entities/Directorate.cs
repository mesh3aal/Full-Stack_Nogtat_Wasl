namespace Domain_Layer.Entities;

public class Directorate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<District> Neighborhoods { get; set; } = new List<District>();
}
