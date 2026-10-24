namespace Application_Layer.Models.RequestDTO.ReviewController
{
    public class ReviewRequest
    {
        [Required(ErrorMessage = "التقييم مطلوب")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public sbyte RateValue { get; set; }

        [MaxLength(1000, ErrorMessage = "التعليق لا يمكن أن يتجاوز 1000 حرفاً")]
        public string? Note { get; set; }
    }
}
