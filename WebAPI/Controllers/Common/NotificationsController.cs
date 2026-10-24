using Application_Layer.Models.ResponseDTO.NotificationsController;
using Application_Layer.Services.Notifications;

namespace WebAPI.Controllers.Common;

[ApiController]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpGet(Routing.Notification.Notifications)]

    public async Task<Result<ListNotifications<NotificationResponse>>> GetNotification(int pageNumber = 1, int pageSize = 10)
    {
        return await notificationService.GetNotification(pageNumber, pageSize);
    }
}