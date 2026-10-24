using System.ComponentModel.DataAnnotations.Schema;

namespace Domain_Layer.Entities;

[Table("notifications")]
public record Notification
{
    [Key]
    [Column("Id")]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    [Required]
    [Column("Title")]
    public string Title { get; set; } = null!;
    
    [Required]
    [Column("Body")]
    public string Body { get; set; } = null!;
    
    
    [Column("Created_At")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // relations
    [Required]
    public string UserId { get; set; } = null!;
    
    public User? User { get; set; }
}