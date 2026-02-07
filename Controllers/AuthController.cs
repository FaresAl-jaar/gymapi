using GymApi.DTOs;
using GymApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
            return BadRequest(new { error = "Username muss mindestens 3 Zeichen haben" });

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            return BadRequest(new { error = "Ungültige E-Mail-Adresse" });

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { error = "Passwort muss mindestens 6 Zeichen haben" });

        var result = await _authService.RegisterAsync(request);
        
        if (result == null)
            return Conflict(new { error = "E-Mail oder Username bereits vergeben" });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result == null)
            return Unauthorized(new { error = "E-Mail oder Passwort falsch" });

        return Ok(result);
    }
}
