using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application_Layer.Models.ReciveDTOs.DashboardController.OwnerDasboard
{
    public class CreatePlaceOwnerDto
    {
        [Required(ErrorMessage = "اسم المكان مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم المكان لا يمكن أن يتجاوز 200 حرفاً")]
        public string PlaceName { get; set; } = null!;

        [Required(ErrorMessage = "رقم السجل التجاري مطلوب")]
        [MaxLength(50, ErrorMessage = "رقم السجل التجاري لا يمكن أن يتجاوز 50 حرفاً")]
        public string CommercialRegisterNumber { get; set; } = null!;

        [Required(ErrorMessage = "اسم الشركة مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم الشركة لا يمكن أن يتجاوز 200 حرفاً")]
        public string CompanyName { get; set; } = null!;


        // Location
        [Required(ErrorMessage = "أقرب معلم مطلوب")]
        [MaxLength(200, ErrorMessage = "أقرب معلم لا يمكن أن يتجاوز 200 حرفاً")]
        public string NearestLandmark { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required(ErrorMessage = "معرف المديرية مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف المديرية غير صحيح")]
        public int DirectorateId { get; set; }

        [Required(ErrorMessage = "معرف المنطقة مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف المنطقة غير صحيح")]
        public int DistrictId { get; set; }

        public TimeOnly? OpeningTime { get; set; }
        public TimeOnly? ClosingTime { get; set; }

    
        [Required(ErrorMessage = "معرف فئة الفرعي مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفئة الفرعي غير صحيح")]
        public int SubCategoryId { get; set; }



        [Required(ErrorMessage = "معرف فئة الرئيسي مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفئة الرئيسي غير صحيح")]
        public int MainCategoryId { get; set; }


        // Details
        [MaxLength(2000, ErrorMessage = "الوصف لا يمكن أن يتجاوز 2000 حرفاً")]
        public string? Description { get; set; }

        // Contact
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public List<string> PhoneNumber { get; set; } = null!;

        // Images
        [Required(ErrorMessage = "صورة السجل التجاري مطلوبة")]
        public IFormFile ImagePathCommercialRegister { get; set; } = null!;
        public List<IFormFile>? Images { get; set; }
    }
}
