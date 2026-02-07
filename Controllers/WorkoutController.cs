using System.Security.Claims;
using GymApi.Data;
using GymApi.DTOs;
using GymApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutController : ControllerBase
{
    private readonly GymDbContext _db;

    public WorkoutController(GymDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("active")]
    public async Task<ActionResult<List<WorkoutSessionDto>>> GetActiveSessions()
    {
        var currentUser = await _db.Users.FindAsync(GetUserId());
        
        var sessions = await _db.WorkoutSessions
            .Include(s => s.User)
            .Where(s => s.EndTime == null)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

        var result = sessions.Select(s => new WorkoutSessionDto(
            s.Id,
            s.UserId,
            s.User.Username,
            s.WorkoutType,
            s.CurrentSet,
            s.TotalSets,
            s.CurrentExercise,
            s.StartTime,
            CalculateDistance(currentUser, s.User)
        )).ToList();

        return Ok(result);
    }

    [HttpPost("checkin")]
    public async Task<ActionResult<WorkoutSessionDto>> CheckIn([FromBody] CheckInRequest request)
    {
        var userId = GetUserId();
        
        // End any existing active session
        var existingSession = await _db.WorkoutSessions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.EndTime == null);
        
        if (existingSession != null)
        {
            existingSession.EndTime = DateTime.UtcNow;
        }

        var session = new WorkoutSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkoutType = request.WorkoutType,
            StartTime = DateTime.UtcNow
        };

        _db.WorkoutSessions.Add(session);
        
        // Update last workout
        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastWorkout = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return Ok(new WorkoutSessionDto(
            session.Id,
            session.UserId,
            user?.Username ?? "Unknown",
            session.WorkoutType,
            session.CurrentSet,
            session.TotalSets,
            session.CurrentExercise,
            session.StartTime,
            null
        ));
    }

    [HttpPost("checkout/{sessionId}")]
    public async Task<ActionResult> CheckOut(Guid sessionId)
    {
        var session = await _db.WorkoutSessions.FindAsync(sessionId);
        
        if (session == null)
            return NotFound();

        if (session.UserId != GetUserId())
            return Forbid();

        session.EndTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{sessionId}")]
    public async Task<ActionResult> UpdateSession(Guid sessionId, [FromBody] UpdateSessionRequest request)
    {
        var session = await _db.WorkoutSessions.FindAsync(sessionId);
        
        if (session == null)
            return NotFound();

        if (session.UserId != GetUserId())
            return Forbid();

        session.CurrentSet = request.CurrentSet;
        session.TotalSets = request.TotalSets;
        session.CurrentExercise = request.CurrentExercise;
        
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("stoke/{targetUserId}")]
    public async Task<ActionResult> SendStoke(Guid targetUserId)
    {
        var fromUserId = GetUserId();
        
        if (fromUserId == targetUserId)
            return BadRequest(new { error = "Du kannst dir nicht selbst einen Stoke senden" });

        var stoke = new Stoke
        {
            Id = Guid.NewGuid(),
            FromUserId = fromUserId,
            ToUserId = targetUserId,
            SentAt = DateTime.UtcNow
        };

        _db.Stokes.Add(stoke);
        await _db.SaveChangesAsync();

        return Ok();
    }

    private static double? CalculateDistance(User? currentUser, User targetUser)
    {
        if (currentUser?.Latitude == null || currentUser.Longitude == null ||
            targetUser.Latitude == null || targetUser.Longitude == null)
            return null;

        // Haversine formula
        const double R = 6371; // Earth's radius in km
        var lat1 = currentUser.Latitude.Value * Math.PI / 180;
        var lat2 = targetUser.Latitude.Value * Math.PI / 180;
        var dLat = (targetUser.Latitude.Value - currentUser.Latitude.Value) * Math.PI / 180;
        var dLon = (targetUser.Longitude.Value - currentUser.Longitude.Value) * Math.PI / 180;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return Math.Round(R * c, 1);
    }
}
