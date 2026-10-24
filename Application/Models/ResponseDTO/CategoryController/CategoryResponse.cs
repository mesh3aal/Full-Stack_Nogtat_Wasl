namespace Application_Layer.Models.SendDTO.CategoryController
{
    public class CategoryResponse
    {
        public int id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIcon { get; set; } = string.Empty;
        public string? Source { get; set; } = string.Empty;
        public List<SubCategoryResponse>? SubCategory { get; set; }

    }
}
