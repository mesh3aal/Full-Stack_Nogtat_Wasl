
namespace Application_Layer.Models.RequestDTO.CategoryController
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "اسم القسم مطلوب")]
        [MaxLength(150, ErrorMessage = "اسم القسم لا يمكن أن يتجاوز 150 حرفاً")]
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIcon { get; set; } 
        public string? Source { get; set; }
        public List<SupCategoryCreate> SupCategory { get; set; } = new List<SupCategoryCreate>();

        public static Category ToEntity(CreateCategoryDto dto)
        {
            return new Category
            {
                CategoryName = dto.CategoryName,
                CategoryIcon = dto.CategoryIcon,
                Source = dto.Source ?? string.Empty
            };
        }



    }
}
