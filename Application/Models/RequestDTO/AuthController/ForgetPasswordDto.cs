namespace Application_Layer.Models.ReciveDTOs.AuthController;

public record ForgetPasswordDto
{
    [Required(ErrorMessage = "عفواً، البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "الرجاء إدخال بريد إلكتروني صحيح")]
    [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يمكن أن يتجاوز 100 حرفاً")]
    public string Email { get; set; } = null!;
}