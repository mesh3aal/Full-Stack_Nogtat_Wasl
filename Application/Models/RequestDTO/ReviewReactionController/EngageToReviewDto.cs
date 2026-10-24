namespace Application_Layer.Models.RequestDTO.ReviewReactionController
{
    public class EngageToReviewDto
    {
        [Required(ErrorMessage = "نوع التفاعل مطلوب")]
        public ReactionType ReactionType { get; set; }
    }
}
