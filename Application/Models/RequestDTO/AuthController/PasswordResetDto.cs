namespace Application_Layer.Models.RequestDTO.AuthController;

public record PasswordResetDto
{
    [Required(ErrorMessage = "الرجاء ادخال كلمة السر")]
    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "كلمة المرور لا يمكن أن تتجاوز 100 حرفاً")]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = "الرجاء ادخل تأكيد كلمة السر الجديدة")]
    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "تأكيد كلمة المرور لا يمكن أن يتجاوز 100 حرفاً")]
    [Compare(nameof(Password), ErrorMessage = "كلمة المرور وتأكيد كلمة المرور لا يتطابقان.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "البريد الالكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "الرجاء إدخال بريد إلكتروني صحيح")]
    [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يمكن أن يتجاوز 100 حرفاً")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "التوكين مطلوبة")]
    public string Pin { get; set; } = null!;
}