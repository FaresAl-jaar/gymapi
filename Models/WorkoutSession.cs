namespace GymApi.Models;

public class WorkoutSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string WorkoutType { get; set; } = string.Empty;
    public int CurrentSet { get; set; } = 1;
    public int TotalSets { get; set; } = 5;
    public string? CurrentExercise { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public bool IsActive => EndTime == null;
    
    // Navigation
    public User User { get; set; } = null!;
}
