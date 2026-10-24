
using Application_Layer.Models.RequestDTO.ReviewController;
using Application_Layer.Models.RequestDTO.ReviewReactionController;
using Application_Layer.Models.ResponseDTO.PlaceController;
using Application_Layer.Models.ResponseDTO.ReviewController;
using Application_Layer.Models.ResponseDTO.ReviewReactionController;

namespace Application_Layer.Services.InteractionsAndFeedback;

public interface IReviewService
{
    Task<Result<ListReviews<ReviewResponse>>> GetPlaceReviews(int placeId, int pageNumber = 1,int pageSize = 10);
    Task<Result<List<ReviewResponse>>> GetMyReviews(int pageNumber = 1, int pageSize = 10);
    Task<Result<ReviewResponse>> GetReview(int reviewId);
    Task<Result<ReviewResponse>> AddReview(int placeId, ReviewRequest model);
    Task<Result<ReviewEditRequest>> EditReview(int reviewId, ReviewEditRequest model);
    Task<Result> DeleteReview(int reviewId);
    Task<Result<ReviewIdResponse>?> IsUserHaveReviewOnPlace(int placeId);
    Task<Result<ReviewReactionDto>> EngageToReview(int reviewId, EngageToReviewDto model);
    Task<Result<ReactionType>> EngagementType(int reviewId);
    //Task<Result<List<IntractionTypes>>> EngagementsType(int placeId);
}
