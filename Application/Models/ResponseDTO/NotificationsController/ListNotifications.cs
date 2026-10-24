namespace Application_Layer.Models.ResponseDTO.NotificationsController;

public record ListNotifications<T>
{
    public List<T> Notifications { get; set; } = new List<T>();
    public int RemainingPlacesCount { get; set; }
}