namespace Application_Layer.Models.ResponseDTO.NotificationsController;

public sealed record NotificationResponse
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime NotificationDate { get; set; }
}