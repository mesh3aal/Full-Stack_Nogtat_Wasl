
namespace Application_Layer.Models.ReciveDTOs.CategoryDTOs
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIcon { get; set; } = string.Empty;
        public string? Source { get; set; } = string.Empty;


        public List<SupCategoryUpdatedto> SupCategory { get; set; } = new List<SupCategoryUpdatedto>();


        public static Category UpdateEntity(Category category, UpdateCategoryDto dto)
        {

            category.CategoryName = dto.CategoryName;
            category.CategoryIcon = dto.CategoryIcon;
            if (!string.IsNullOrEmpty(dto.Source))
            {
                category.Source = dto.Source;
            }
            return category;

        }
    }
}
