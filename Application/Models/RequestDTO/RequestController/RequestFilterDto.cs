namespace Application_Layer.Models.RequestDTO.RequestController
{
    public class RequestFilterDto
    {
        [MaxLength(100, ErrorMessage = "البحث لا يمكن أن يتجاوز 100 حرفاً")]
        public string? Search { get; set; }
        
        public RequestStates? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "رقم الصفحة يجب أن يكون 1 أو أكثر")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "حجم الصفحة يجب أن يكون بين 1 و 100")]
        public int PageSize { get; set; } = 10;
    }
}
