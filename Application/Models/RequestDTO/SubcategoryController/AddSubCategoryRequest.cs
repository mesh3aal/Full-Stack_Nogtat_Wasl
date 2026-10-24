using System.ComponentModel.DataAnnotations;

namespace Application_Layer.Models.ReciveDTOs.SubcategoryController
{
    public class AddSubCategoryRequest
    {
        [Required(ErrorMessage = "اسم القسم الفرعي مطلوب")]
        [MaxLength(150, ErrorMessage = "اسم القسم الفرعي لا يمكن أن يتجاوز 150 حرفاً")]
        public string SubCategoryName { get; set; } = null!;

        [Required(ErrorMessage = "معرف الفئة الرئيسي مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفئة الرئيسي غير صحيح")]
        public int CategoryId { get; set; }
    }
}
