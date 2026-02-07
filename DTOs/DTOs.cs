namespace GymApi.DTOs;

// ===== Auth =====
public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(Guid UserId, string Username, string Email, string Token);

// ===== User =====
public record UserDto(Guid Id, string Username, string AvatarEmoji, DateTime? LastWorkout, bool IsSlacking);
public record UpdateLocationRequest(double Latitude, double Longitude);

// ===== Workout Session =====
public record WorkoutSessionDto(
    Guid Id, 
    Guid UserId, 
    string UserName,
    string WorkoutType, 
    int CurrentSet, 
    int TotalSets, 
    string? CurrentExercise,
    DateTime StartTime,
    double? DistanceKm
);
public record CheckInRequest(string WorkoutType);
public record UpdateSessionRequest(int CurrentSet, int TotalSets, string? CurrentExercise);

// ===== Exercise =====
public record ExerciseDto(Guid Id, string Name, string Category);
public record CreateExerciseRequest(string Name, string Category);

// ===== Personal Record =====
public record PersonalRecordDto(
    Guid Id,
    Guid UserId,
    string Username,
    Guid ExerciseId,
    string ExerciseName,
    double Weight,
    int Reps,
    DateTime AchievedAt,
    bool IsNewPR
);
public record LogPRRequest(Guid ExerciseId, double Weight, int Reps);

// ===== Leaderboard =====
public record LeaderboardEntryDto(
    int Rank,
    Guid UserId,
    string Username,
    string AvatarEmoji,
    int WorkoutsThisWeek,
    int Points,
    int DaysSinceLastWorkout,
    bool IsSlacking
);
