namespace Application_Layer.Models.RequestDTO.UserManagementController.UserManagementDto
{
    public class CreateNewUserDto
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MaxLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرفاً")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يمكن أن يتجاوز 100 حرفاً")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "كلمة المرور لا يمكن أن تتجاوز 100 حرفاً")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور لا يتطابقان")]
        public string ConfirmPassword { get; set; } = null!;

        public UserType UserType { get; set; }
    }
}
