namespace GymApi.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string AvatarEmoji { get; set; } = "???";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastWorkout { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? LocationUpdatedAt { get; set; }
    
    // Navigation
    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = [];
    public ICollection<PersonalRecord> PersonalRecords { get; set; } = [];
    public ICollection<Exercise> CreatedExercises { get; set; } = [];
}
