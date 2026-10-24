namespace Domain_Layer.Entities;

public class SubCategory
{
    public int SubCategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? supCategoryIcon { get; set; }=string.Empty;
    //
    //
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Place> Places{ get; } = new List<Place>();
}
