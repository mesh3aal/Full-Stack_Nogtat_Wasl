namespace Application_Layer.Models.RequestDTO.UsersettingsController
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "كلمة المرور الحالية لا يمكن أن تتجاوز 100 حرفاً")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "كلمة المرور الجديدة لا يمكن أن تتجاوز 100 حرفاً")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "تأكيد كلمة المرور لا يمكن أن يتجاوز 100 حرفاً")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور لا يتطابقان")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
