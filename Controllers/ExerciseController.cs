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
public class ExerciseController : ControllerBase
{
    private readonly GymDbContext _db;

    public ExerciseController(GymDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<ExerciseDto>>> GetExercises()
    {
        var userId = GetUserId();
        
        var exercises = await _db.Exercises
            .Where(e => e.IsGlobal || e.CreatedByUserId == userId)
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .Select(e => new ExerciseDto(e.Id, e.Name, e.Category))
            .ToListAsync();

        return Ok(exercises);
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> CreateExercise([FromBody] CreateExerciseRequest request)
    {
        var exercise = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Category = request.Category,
            CreatedByUserId = GetUserId(),
            IsGlobal = false
        };

        _db.Exercises.Add(exercise);
        await _db.SaveChangesAsync();

        return Ok(new ExerciseDto(exercise.Id, exercise.Name, exercise.Category));
    }
}
