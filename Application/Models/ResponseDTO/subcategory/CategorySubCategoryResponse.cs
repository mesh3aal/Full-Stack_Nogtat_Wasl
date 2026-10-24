namespace Application_Layer.Models.SendDTO.subcategory;

public record CategorySubCategoryResponse
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public List<SubCategoryResponse> SubCategories { get; set; } = new List<SubCategoryResponse>();
}
