using Application_Layer.Models.ResponseDTO.NotificationsController;
using Application_Layer.Services.IdentityAndAccess;

namespace Application_Layer.Services.Notifications;

public class NotificationService(ApplicationDbContext context, ICurrentUserService userService) : INotificationService
{
    public async Task<Result<ListNotifications<NotificationResponse>>> GetNotification(int pageNumber=1, int pageSize=10)
    {
        var user = await userService.GetUserAsync();
        if (user == null) return Result<ListNotifications<NotificationResponse>>.Unauthorized("الرجاء قم بتسجيل الدخول");

        var query = context.Notifications.Where(x => x.UserId == user.Id);
        
        var skip = (pageNumber - 1) * pageSize;
        var take = pageSize;
        var remaining = await query.CountAsync() - (skip + take) < 0 ? 0 : await query.CountAsync() - (skip + take);

        var data = await query.Skip(skip).Take(take).Select(x => new NotificationResponse
        {
            Title = x.Title,
            Body = x.Body,
            NotificationDate = x.CreatedAt
        }).ToListAsync();

        var final = new ListNotifications<NotificationResponse>
        {
            Notifications = data,
            RemainingPlacesCount = remaining
        };
        return Result<ListNotifications<NotificationResponse>>.Success(final);
    }
}