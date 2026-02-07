using System.Security.Claims;
using GymApi.Data;
using GymApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaderboardController : ControllerBase
{
    private readonly GymDbContext _db;

    public LeaderboardController(GymDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard()
    {
        var weekStart = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        
        // Get users with their workout counts this week
        var usersWithWorkouts = await _db.Users
            .Select(u => new
            {
                User = u,
                WorkoutsThisWeek = u.WorkoutSessions.Count(ws => ws.StartTime >= weekStart),
                PRsThisWeek = u.PersonalRecords.Count(pr => pr.AchievedAt >= weekStart && pr.IsNewPR),
                DaysSinceLastWorkout = u.LastWorkout.HasValue 
                    ? (int)(DateTime.UtcNow - u.LastWorkout.Value).TotalDays 
                    : 999
            })
            .OrderByDescending(x => x.WorkoutsThisWeek * 100 + x.PRsThisWeek * 50)
            .Take(50)
            .ToListAsync();

        var result = usersWithWorkouts.Select((x, index) => new LeaderboardEntryDto(
            Rank: index + 1,
            UserId: x.User.Id,
            Username: x.User.Username,
            AvatarEmoji: x.User.AvatarEmoji,
            WorkoutsThisWeek: x.WorkoutsThisWeek,
            Points: x.WorkoutsThisWeek * 100 + x.PRsThisWeek * 50,
            DaysSinceLastWorkout: x.DaysSinceLastWorkout,
            IsSlacking: x.DaysSinceLastWorkout >= 4
        )).ToList();

        return Ok(result);
    }
}
