namespace Application_Layer.Models.SendDTO.CategoryController
{
    public class CategoryListResponse
    {
        public List<CategoryResponse> categories { get; set; } = new List<CategoryResponse>();
        public int totalCount { get; set; }
    }
}
