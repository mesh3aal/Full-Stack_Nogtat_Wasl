namespace Application_Layer.Models.SendDTO.subcategory;

public record SubCategoryResponse
{
    public int  SubCategoryId { get; set; }
    public string SubCategoryName { get; set; } = null!;
    public string? SubCategoryIcon { get; set; }

}
