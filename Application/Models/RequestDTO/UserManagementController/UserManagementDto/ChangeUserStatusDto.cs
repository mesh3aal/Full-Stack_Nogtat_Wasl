using System.ComponentModel.DataAnnotations;

namespace Application_Layer.Models.ReciveDTOs.UserManagementController.UserManagementDto
{
    public class ChangeUserStatusDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "الحالة الجديدة مطلوبة")]
        public UserStatus Status { get; set; }
    }
}
