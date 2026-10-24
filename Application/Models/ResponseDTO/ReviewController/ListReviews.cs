namespace Application_Layer.Models.ResponseDTO.ReviewController;

public record ListReviews<T>
{
    public List<T> Reviews { get; set; } = new List<T>();
    public int RemainingReviewsCount { get; set; }
}