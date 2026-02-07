using GymApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Data;

public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<PersonalRecord> PersonalRecords => Set<PersonalRecord>();
    public DbSet<Stoke> Stokes => Set<Stoke>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // WorkoutSession
        modelBuilder.Entity<WorkoutSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.WorkoutSessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Exercise
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.CreatedByUser)
                .WithMany(u => u.CreatedExercises)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PersonalRecord
        modelBuilder.Entity<PersonalRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.PersonalRecords)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Exercise)
                .WithMany(e => e.PersonalRecords)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Stoke
        modelBuilder.Entity<Stoke>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.FromUser)
                .WithMany()
                .HasForeignKey(e => e.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ToUser)
                .WithMany()
                .HasForeignKey(e => e.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed default exercises
        SeedExercises(modelBuilder);
    }

    private static void SeedExercises(ModelBuilder modelBuilder)
    {
        var exercises = new[]
        {
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Bench Press", Category = "Chest", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Incline Bench Press", Category = "Chest", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Squat", Category = "Legs", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Deadlift", Category = "Back", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Overhead Press", Category = "Shoulders", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "Barbell Row", Category = "Back", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "Pull-Up", Category = "Back", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Name = "Dips", Category = "Chest", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Name = "Leg Press", Category = "Legs", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), Name = "Romanian Deadlift", Category = "Legs", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), Name = "Bicep Curl", Category = "Arms", IsGlobal = true },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), Name = "Tricep Extension", Category = "Arms", IsGlobal = true },
        };

        modelBuilder.Entity<Exercise>().HasData(exercises);
    }
}
