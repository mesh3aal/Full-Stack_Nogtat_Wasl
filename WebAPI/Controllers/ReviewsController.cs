using System.ComponentModel.DataAnnotations;
using Application_Layer.Models.RequestDTO.ReviewController;
using Application_Layer.Models.RequestDTO.ReviewReactionController;
using Application_Layer.Models.ResponseDTO.ReviewController;
using Application_Layer.Models.ResponseDTO.ReviewReactionController;
using Application_Layer.Services.InteractionsAndFeedback;

namespace WebAPI.Controllers;

[ApiController]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpGet(Routing.Reviews.GetPlaceReviews)]
    public async Task<Result<ListReviews<ReviewResponse>>> GetPlaceReviews(int placeId, int pageNumber = 1, int pageSize = 10)
    {
        return await reviewService.GetPlaceReviews(placeId, pageNumber, pageSize);
    }
    
    [TranslateResultToActionResult]
    [HttpGet(Routing.Reviews.GetMyReviews)]
    public async Task<Result<List<ReviewResponse>>> GetMyReviews(int pageNumber = 1,int pageSize = 10)
    {
        return await reviewService.GetMyReviews(pageNumber,pageSize);
    }
    
    [TranslateResultToActionResult]
    [HttpGet(Routing.Reviews.GetReview)]
    public async Task<Result<ReviewResponse>> GetReview(int reviewId)
    {
        return await reviewService.GetReview(reviewId);
    }
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.Reviews.AddReview)]
    public async Task<Result<ReviewResponse>> AddReview(int placeId,[FromBody]ReviewRequest model)
    {
        return await reviewService.AddReview(placeId, model);
    }
    
    [TranslateResultToActionResult]
    [HttpPatch(Routing.Reviews.EditReview)]
    public async Task<Result<ReviewEditRequest>> EditReview(int reviewId, ReviewEditRequest model)
    {
        return await reviewService.EditReview(reviewId, model);
    }
    
    [TranslateResultToActionResult]
    [HttpDelete(Routing.Reviews.DeleteReview)]
    public async Task<Result> DeleteReview([Required(ErrorMessage = "يجب إدخال المعرف الخاص بالتعليق")]int reviewId)
    {
        return await reviewService.DeleteReview(reviewId);
    }
    [TranslateResultToActionResult]
    [HttpGet(Routing.Reviews.isUserHaveReviewOnPlace)]
    public async Task<Result<ReviewIdResponse>?> IsUserHaveReviewOnPlace(int placeId)
    {
        return await reviewService.IsUserHaveReviewOnPlace(placeId);
    }


    [TranslateResultToActionResult]
    [HttpPost(Routing.Reviews.EngageToReview)]
    public async Task<Result<ReviewReactionDto>> EngageToReview([Required(ErrorMessage = "يجب إدخال المعرف الخاص بالتعليق")]int reviewId, EngageToReviewDto model)
    {
        return await  reviewService.EngageToReview(reviewId, model);
    }
    [TranslateResultToActionResult]
    [HttpGet(Routing.Reviews.EngagementType)]
    public async Task<Result<ReactionType>> EngagementType([Required(ErrorMessage = "يجب إدخال المعرف الخاص بالتعليق")]int reviewId)
    {
        return await reviewService.EngagementType(reviewId);
    }
}
