namespace Application_Layer.Models.RequestDTO.UserManagementController.UserManagementDto
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MaxLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرفاً")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يمكن أن يتجاوز 100 حرفاً")]
        public string Email { get; set; } = null!;
   
        public int UserStatus { get; set; }

        public UserType UserType { get; set; }
    }
}
