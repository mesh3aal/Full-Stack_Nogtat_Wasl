namespace Domain_Layer.Entities;

public class District
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public int DirectorateId { get; set; }
    public Directorate Directorate { get; set; } = null!;
}
