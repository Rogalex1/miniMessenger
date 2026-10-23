using Microsoft.AspNetCore.Mvc;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result is null)
            return Conflict(new { message = "Ce nom d'utilisateur existe déjà." });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result is null)
            return Unauthorized(new { message = "Identifiants invalides." });

        return Ok(result);
    }
}