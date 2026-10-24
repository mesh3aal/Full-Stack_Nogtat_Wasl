
namespace Domain_Layer.Entities;

public class  Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryIcon { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;

    // Relationships
    public ICollection<SubCategory> SubCategories { get; } = new List<SubCategory>();
    public ICollection<Place> Place { get; } = new List<Place>();
};
