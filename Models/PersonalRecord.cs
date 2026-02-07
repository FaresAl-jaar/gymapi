namespace GymApi.Models;

public class PersonalRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExerciseId { get; set; }
    public double Weight { get; set; }
    public int Reps { get; set; }
    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
    public bool IsNewPR { get; set; } = true;
    
    // Navigation
    public User User { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
}
