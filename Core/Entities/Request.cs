
namespace Domain_Layer.Entities;

public class Request
{
    public int RequestId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public RequestStates State { get; set; }
    public string? RejectionReason { get; set; }
    public Place Place { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}
