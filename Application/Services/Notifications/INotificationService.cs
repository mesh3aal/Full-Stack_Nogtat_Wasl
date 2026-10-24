using Application_Layer.Models.ResponseDTO.NotificationsController;

namespace Application_Layer.Services.Notifications;

public interface INotificationService
{
    Task<Result<ListNotifications<NotificationResponse>>> GetNotification(int pageNumber, int pageSize); 
}