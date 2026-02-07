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
public class PRController : ControllerBase
{
    private readonly GymDbContext _db;

    public PRController(GymDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("feed")]
    public async Task<ActionResult<List<PersonalRecordDto>>> GetPRFeed()
    {
        var records = await _db.PersonalRecords
            .Include(pr => pr.User)
            .Include(pr => pr.Exercise)
            .OrderByDescending(pr => pr.AchievedAt)
            .Take(50)
            .Select(pr => new PersonalRecordDto(
                pr.Id,
                pr.UserId,
                pr.User.Username,
                pr.ExerciseId,
                pr.Exercise.Name,
                pr.Weight,
                pr.Reps,
                pr.AchievedAt,
                pr.IsNewPR
            ))
            .ToListAsync();

        return Ok(records);
    }

    [HttpPost]
    public async Task<ActionResult<PersonalRecordDto>> LogPR([FromBody] LogPRRequest request)
    {
        var userId = GetUserId();
        var user = await _db.Users.FindAsync(userId);
        var exercise = await _db.Exercises.FindAsync(request.ExerciseId);

        if (exercise == null)
            return NotFound(new { error = "Übung nicht gefunden" });

        // Check if this is actually a new PR
        var existingPR = await _db.PersonalRecords
            .Where(pr => pr.UserId == userId && pr.ExerciseId == request.ExerciseId)
            .OrderByDescending(pr => pr.Weight)
            .ThenByDescending(pr => pr.Reps)
            .FirstOrDefaultAsync();

        var isNewPR = existingPR == null || 
                      request.Weight > existingPR.Weight ||
                      (request.Weight == existingPR.Weight && request.Reps > existingPR.Reps);

        var pr = new PersonalRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ExerciseId = request.ExerciseId,
            Weight = request.Weight,
            Reps = request.Reps,
            AchievedAt = DateTime.UtcNow,
            IsNewPR = isNewPR
        };

        _db.PersonalRecords.Add(pr);
        await _db.SaveChangesAsync();

        return Ok(new PersonalRecordDto(
            pr.Id,
            pr.UserId,
            user?.Username ?? "Unknown",
            pr.ExerciseId,
            exercise.Name,
            pr.Weight,
            pr.Reps,
            pr.AchievedAt,
            pr.IsNewPR
        ));
    }
}
