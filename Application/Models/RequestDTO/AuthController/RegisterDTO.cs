namespace Application_Layer.Models.ReciveDTOs.AuthController;

public record RegisterDto
{
    [Required(ErrorMessage = "عفواً، اسم المستخدم مطلوب")]
    [MaxLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرفاً")]
    public string Username { get; set; } = null!;
    [EmailAddress(ErrorMessage = "الرجاء إدخال بريد إلكتروني صحيح")]
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يمكن أن يتجاوز 100 حرفاً")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "يرجى ادخال كلمة المرور، هذا الحقل لا يمكن ان يكون فارغاً.")]
    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "كلمة المرور لا يمكن أن تتجاوز 100 حرفاً")]
    public string Password { get; set; } = null!;
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "حقل كلمة المرور و حقل تأكيد كلمة المرور لايتطابقان يرجى ادخال نفس كلمة المرور.")]
    public string ConfirmPassword { get; set; } = null!;
    public UserType UserType { get; set; }
}