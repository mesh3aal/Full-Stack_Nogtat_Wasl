
namespace Application_Layer.Models.ReciveDTOs.CategoryController;

public record AddCategoryRequest
{
    [Required(ErrorMessage = "الرجاء إدخال اسم الصنف.")]
    public string CategoryName { get; set; } = null!;
}
