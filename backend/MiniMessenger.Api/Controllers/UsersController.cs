using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Extensions;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userService.GetOtherUsersAsync(User.GetUserId());
        return Ok(users);
    }
}