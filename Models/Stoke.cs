namespace GymApi.Models;

public class Stoke
{
    public Guid Id { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User FromUser { get; set; } = null!;
    public User ToUser { get; set; } = null!;
}
