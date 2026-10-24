using Application_Layer.Models.RequestDTO.ReviewController;
using Application_Layer.Models.RequestDTO.ReviewReactionController;
using Application_Layer.Models.ResponseDTO.ReviewController;
using Application_Layer.Models.ResponseDTO.ReviewReactionController;
using Application_Layer.Services.IdentityAndAccess;

namespace Application_Layer.Services.InteractionsAndFeedback;

public class ReviewService(
    ApplicationDbContext context,
    ICurrentUserService currentUserService) : IReviewService
{
    public async Task<Result<ListReviews<ReviewResponse>>> GetPlaceReviews(int placeId, int pageNumber = 1, int pageSize = 10)
    {
        var userId = currentUserService.GetUserId();
        var reviews = context.Reviews.Include(x => x.Place)
            .Where(x => x.PlaceId == placeId && x.Place.State == PlaceStatus.Active);
        var take = pageSize;
        var skip = (pageNumber - 1) * pageSize;
        var remaining = await reviews.CountAsync() - (skip + take) < 0 ? 0 : await reviews.CountAsync() - (skip + take);

        var data  = await reviews.Select(x => new ReviewResponse
        {
            ReviewId = x.ReviewId,
            PlaceName = x.Place.PlaceName,
            Note = x.Note,
            RateValue = x.RateValue,
            CreatedAt = x.CreatedAt,
            User = new UserInfoRate
            {
                UserName = x.User.UserName!,
                UserAvatar = x.User.Avatar!,
            },
            ReactionType = userId == null ? ReviewReactionStatus.NotExists
                : x.ReviewReactions.Any(r => r.UserId == userId && r.Type == ReactionType.Like) ? ReviewReactionStatus.Liked
                : x.ReviewReactions.Any(r => r.UserId == userId && r.Type == ReactionType.Dislike) ? ReviewReactionStatus.Disliked
                : ReviewReactionStatus.NotExists,
            ReactionCount = new ReviewReactionCount
            {
                Likes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Like).Count(),
                Dislikes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Dislike).Count()
            }
        }).AsQueryable().AsSplitQuery().Skip(skip).Take(take).ToListAsync();
        ListReviews<ReviewResponse> final = new ListReviews<ReviewResponse>
        {
            Reviews = data,
            RemainingReviewsCount = remaining
        };
        return  Result<ListReviews<ReviewResponse>>.Success(final);
    }

    public async Task<Result<List<ReviewResponse>>> GetMyReviews(int pageNumber = 1, int pageSize = 10)
    {
        var user = await currentUserService.GetUserAsync();
        if(!currentUserService.IsAuthenticatedAsync()) return Result<List<ReviewResponse>>.Unauthorized();

        var reviews = context.Reviews.Include(x => x.User)
            .Where(x => x.UserId == user!.Id)
            .Where(x => x.User.Status == UserStatus.Active);

        var myRates = reviews.Select(x => new ReviewResponse
        {
            ReviewId = x.ReviewId,
            PlaceName = x.Place.PlaceName,
            RateValue = x.RateValue,
            CreatedAt = x.CreatedAt,
            Note = x.Note,
            User = new UserInfoRate
            {
                UserName = x.User.UserName!,
                UserAvatar = x.User.Avatar!,
            },
            ReactionCount = new ReviewReactionCount
            {
                Likes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Like).Count(),
                Dislikes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Dislike).Count()
            }
        });
        var take = pageSize;
        var skip = (pageNumber - 1) * pageSize;
        var remaining = await myRates.CountAsync() - (skip + take) < 0 ? 0 : await myRates.CountAsync() - (skip + take);
            
           var final = await myRates.Skip(skip).Take(take).ToListAsync();
           
        return Result<List<ReviewResponse>>.Success(final);
    }
    
    public async Task<Result<ReviewResponse>> GetReview(int reviewId)
    {
        var review = await context.Reviews.Include(x => x.Place)
            .ThenInclude(x => x.User)
            // الشرط هنا يتطلب ان يكون المستخدم غير محظور ( شغال ) عشان يطلع حقه التعليق
            .Where(x => x.User.Status == UserStatus.Active)
            .Select(x => new ReviewResponse
            {
                ReviewId = x.ReviewId,
                PlaceName = x.Place.PlaceName,
                RateValue = x.RateValue,
                CreatedAt = x.CreatedAt,
                Note = x.Note,
                User = new UserInfoRate
                {
                    UserName = x.User.UserName!,
                    UserAvatar = x.User.Avatar!,
                },
                ReactionCount = new ReviewReactionCount
                {
                    Likes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Like).Count(),
                    Dislikes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Dislike).Count()
                }
            })
            .FirstOrDefaultAsync(x => x.ReviewId == reviewId);
        if (review == null) return Result<ReviewResponse>.NotFound("لم يتم إجاد المطلوب");

        return Result<ReviewResponse>.Success(review);
    }

    public async Task<Result<ReviewResponse>> AddReview([Required(ErrorMessage = "يجب إدخال المعرف الخاص بالمحل")]int placeId, ReviewRequest model)
    {
        var user = await currentUserService.GetUserAsync();
        if (user == null) return Result<ReviewResponse>.Unauthorized();

        var placeExists = await context.Places.AnyAsync(p => p.PlaceId == placeId);
        if (!placeExists)
        {
            return Result.NotFound("المحل المطلوب غير موجود");
        }

        var hasAlreadyRated = await context.Reviews
            .AnyAsync(x => x.PlaceId == placeId && x.UserId == user.Id);

        if (hasAlreadyRated)
        {
            return Result.Conflict("لقد تم إضافة تعليق مسبقاً لهذا المحل الرجاء قم بحذف تعليقك السابق او قم بالتعديل عليه.");
        }

        context.Reviews.Add(new Review
        {
            UserId = user.Id,
            RateValue = model.RateValue,
            Note = model.Note,
            CreatedAt = DateTime.UtcNow,
            PlaceId = placeId,
        });
        var saveResult = await context.SaveChangesAsync() > 0;
        if (!saveResult)
        {
            return Result.Error("لقد حدث خطاء اثناء إضافة تعليقك، لم يتم الحفظ الرجاء قم بالمحاولة مرة اخرى.");
        }

        return Result.Success();
    }

    public async Task<Result<ReviewEditRequest>> EditReview(int reviewId, ReviewEditRequest model)
    {
        if (!currentUserService.IsAuthenticatedAsync())
        {
            return Result<ReviewEditRequest>.Unauthorized();
        }
        var review = await context.Reviews.Include(x => x.User).FirstOrDefaultAsync(x => x.ReviewId == reviewId);

        if (currentUserService.GetUserRole()![0] != "Admin" && review!.User.Id != currentUserService.GetUserId())
        {
            return Result.Forbidden();
        }

        if (review == null)
        {
            return Result.NotFound("التعليق المطلوب غير موجود");
        }

        review.Note = model.Note;
        review.RateValue = model.RateValue;


        var changes = await context.SaveChangesAsync();

        if (changes <= 0)
        {
            return Result.Error("حدث خطاء لم نتمكن من الحفظ");
        }

        return Result.Success(model);
    }

    public async Task<Result> DeleteReview(int reviewId)
    {
        if (!currentUserService.IsAuthenticatedAsync())
        {
            return Result.Unauthorized();
        }
        
        var review = await context.Reviews.Include(x => x.User).FirstOrDefaultAsync(x => x.ReviewId == reviewId);
        
        if (review == null)
        {
            return Result.NotFound("التعليق المطلوب غير موجود");
        }
        if (currentUserService.GetUserId() != review.UserId)
        {
            return Result.Forbidden();
        }

        context.Reviews.Remove(review);
        if (await context.SaveChangesAsync() <= 0)
            return Result.Error("حدث خطاء لم نتمكن من الحفظ");
        await context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result<ReviewIdResponse>?> IsUserHaveReviewOnPlace(int placeId)
    {
        var user = await currentUserService.GetUserAsync();
        if (user == null) return Result<ReviewIdResponse>.Unauthorized();

        var review = await context.Reviews.FirstOrDefaultAsync(x => x.UserId == user.Id && x.PlaceId == placeId);

        if (review == null) return Result<ReviewIdResponse>.NotFound("لم يتم إجاد الطلب");

        return Result<ReviewIdResponse>.Success(new ReviewIdResponse
        {
            ReviewId = review.ReviewId,
        });
    }

    public async Task<Result<ReviewReactionDto>> EngageToReview(int reviewId, EngageToReviewDto model)
    {
        var user = await currentUserService.GetUserAsync();
        if (user == null) return Result<ReviewReactionDto>.Unauthorized();

        // user need to be authenticated to access this endpoint and like a review. up

        var review = context.Reviews.Include(x=> x.Place)
            .FirstOrDefault(r => r.ReviewId == reviewId);

        if (review == null)
        {
            return Result<ReviewReactionDto>.NotFound("التعليق المطلوب غير موجود");
        }
        
        var reviewCount = context.Places.Count(x => x.PlaceId == review.PlaceId);

        if (reviewCount == 0)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title =  "لقد قام احدهم بإضافة اول تقييم على محلك",
                Body = $"المستخدم {user.UserName} قام بإضافة اول تقييم على محلك {review.Place.PlaceName} قم بزيارة قسم التقيمات لترى التقيم المكتوب لك",
                UserId = review.Place.UserId,
                CreatedAt = DateTime.UtcNow
                
            };
            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
        }
        else if (reviewCount > 0)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title =  "لقد قام احدهم بإضافة تقييم على محلك",
                Body = $"المستخدم {user.UserName} قام بإضافة تقييم على محلك {review.Place.PlaceName} قم بزيارة قسم التقيمات لترى التقيم المكتوب لك",
                UserId = review.Place.UserId,
                CreatedAt = DateTime.UtcNow
            };
            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
        }
        
        // what if the review itself not exists ? up

        var reviewReaction = context.ReviewReactions
            .FirstOrDefault(r => r.UserId == user.Id && r.ReviewId == reviewId);

        var countOfReactions = await context.ReviewReactions
            .Where(r => r.ReviewId == reviewId)
            .GroupBy(r => r.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        //getting the reviewReaction. Up

        if (reviewReaction == null)
        {
            ReviewReaction rr = new ReviewReaction
            {
                ReviewId = reviewId,
                Type = model.ReactionType,
                UserId = user.Id
            };
            context.ReviewReactions.Add(rr);
            await context.SaveChangesAsync();
            var counts = await context.ReviewReactions
                .Where(r => r.ReviewId == reviewId)
                .GroupBy(r => r.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();
            
            if (review.UserId != user.Id)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = model.ReactionType == ReactionType.Like ? "لقد قام احدهم بإضافة إعجاب على تعليقك" : "لقد قام احدهم بإضافة عدم إعجاب على تعليقك",
                    Body = $"المستخدم {user.UserName} قام بإضافة تفاعل على تعليقك على محل {review.Place.PlaceName}",
                    UserId = review.UserId,
                    CreatedAt = DateTime.UtcNow
                
                };
                await context.Notifications.AddAsync(notification);
                await context.SaveChangesAsync();
            }
            
            return Result<ReviewReactionDto>.Success(new ReviewReactionDto
            {
                ReviewId = reviewId,
                State = model.ReactionType == ReactionType.Like
                    ? ReviewReactionStatus.Liked
                    : ReviewReactionStatus.Disliked,
                LikesCount = counts.FirstOrDefault(c => c.Type == ReactionType.Like)?.Count ?? 0,
                DislikesCount = counts.FirstOrDefault(c => c.Type == ReactionType.Dislike)?.Count ?? 0,
            });
        }

        // if reviewReaction not found add one and return the State dto to the user. Up

        if (reviewReaction.Type == model.ReactionType)
        {
            context.ReviewReactions.Remove(reviewReaction);
            await context.SaveChangesAsync();
            var counts = await context.ReviewReactions
                .Where(r => r.ReviewId == reviewId)
                .GroupBy(r => r.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();
            
            
            return Result<ReviewReactionDto>.Success(new ReviewReactionDto
                {
                    ReviewId = reviewId,
                    State = model.ReactionType == ReactionType.Like
                        ? ReviewReactionStatus.Unliked
                        : ReviewReactionStatus.Undisliked,
                    LikesCount = counts.FirstOrDefault(c => c.Type == ReactionType.Like)?.Count ?? 0,
                    DislikesCount = counts.FirstOrDefault(c => c.Type == ReactionType.Dislike)?.Count ?? 0,
                }
            );
        }

        // what if current reviewReaction and user engagement was equal? up
        if (reviewReaction.Type != model.ReactionType)
        {
            reviewReaction.Type = model.ReactionType;
            await context.SaveChangesAsync();
            var counts = await context.ReviewReactions
                .Where(r => r.ReviewId == reviewId)
                .GroupBy(r => r.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();
            return Result<ReviewReactionDto>.Success(new ReviewReactionDto
                {
                    ReviewId = reviewId,
                    State = model.ReactionType == ReactionType.Like
                        ? ReviewReactionStatus.Liked
                        : ReviewReactionStatus.Disliked,
                    LikesCount = counts.FirstOrDefault(c => c.Type == ReactionType.Like)?.Count ?? 0,
                    DislikesCount = counts.FirstOrDefault(c => c.Type == ReactionType.Dislike)?.Count ?? 0,
                }
            );
        }
        // what if current ReviewReaction and user engagement wasn't match ? up
        
        return Result<ReviewReactionDto>.Success(new ReviewReactionDto
            {
                ReviewId = reviewId,
                State = ReviewReactionStatus.NotExists,
                LikesCount = countOfReactions.FirstOrDefault(c => c.Type == ReactionType.Like)?.Count ?? 0,
                DislikesCount = countOfReactions.FirstOrDefault(c => c.Type == ReactionType.Dislike)?.Count ?? 0,
            }
        );
    }

    public async Task<Result<ReactionType>> EngagementType(int reviewId)
    {
        var user = await currentUserService.GetUserAsync();
        if (user == null) return Result<ReactionType>.Unauthorized();

        // user need to be authenticated to access this endpoint and like a review. up

        var review = context.Reviews
            .FirstOrDefault(r => r.ReviewId == reviewId);

        if (review == null)
        {
            return Result<ReactionType>.NotFound("لم يتم إجاد التعليق.");
        }

        var reviewReaction = context.ReviewReactions
            .FirstOrDefault(r => r.UserId == user.Id && r.ReviewId == reviewId);

        return reviewReaction == null
            ? Result<ReactionType>.NotFound("لم يتم إجاد التفاعل على التعليق.")
            : Result<ReactionType>.Success(reviewReaction.Type);
    }
    
}
