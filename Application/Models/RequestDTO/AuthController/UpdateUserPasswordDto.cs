namespace Application_Layer.Models.ReciveDTOs.AuthController;

public record UpdateUserPasswordDto
{
    [Required(ErrorMessage = "كلمة المرور السابقة مطلوبة")]
    [MaxLength(100, ErrorMessage = "كلمة المرور السابقة لا يمكن أن تتجاوز 100 حرفاً")]
    public string OldPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "كلمة المرور الجديدة لا يمكن أن تتجاوز 100 حرفاً")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "تأكيد كلمة المرور لا يمكن أن يتجاوز 100 حرفاً")]
    [Compare(nameof(NewPassword), ErrorMessage = "حقل كلمة المرور و حقل تأكيد كلمة المرور لايتطابقان.")]
    public string ConfirmPassword { get; set; } = null!;
}