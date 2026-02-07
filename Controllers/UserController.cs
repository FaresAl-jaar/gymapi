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
public class UserController : ControllerBase
{
    private readonly GymDbContext _db;

    public UserController(GymDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _db.Users.FindAsync(GetUserId());
        
        if (user == null)
            return NotFound();

        var isSlacking = user.LastWorkout.HasValue && 
                        (DateTime.UtcNow - user.LastWorkout.Value).TotalDays >= 4;

        return Ok(new UserDto(
            user.Id,
            user.Username,
            user.AvatarEmoji,
            user.LastWorkout,
            isSlacking
        ));
    }

    [HttpPut("location")]
    public async Task<ActionResult> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        var user = await _db.Users.FindAsync(GetUserId());
        
        if (user == null)
            return NotFound();

        user.Latitude = request.Latitude;
        user.Longitude = request.Longitude;
        user.LocationUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok();
    }
}
