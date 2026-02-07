namespace GymApi.Models;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Chest, Legs, Back, Shoulders, Arms
    public Guid? CreatedByUserId { get; set; }
    public bool IsGlobal { get; set; } = false; // Vordefinierte Übungen
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User? CreatedByUser { get; set; }
    public ICollection<PersonalRecord> PersonalRecords { get; set; } = [];
}
