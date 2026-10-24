using System.ComponentModel.DataAnnotations;

namespace Application_Layer.Models.ReciveDTOs.UsersettingsController
{
    public class UserProfileChangeDto
    {
        [MaxLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرفاً")]
        public string? UserName { get; set; }

        [MaxLength(500, ErrorMessage = "الوصف الشخصي لا يمكن أن يتجاوز 500 حرفاً")]
        public string? Bio { get; set; } = string.Empty;
    }
}
