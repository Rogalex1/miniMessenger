using Microsoft.EntityFrameworkCore;
using MiniMessenger.Api.Data;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetOtherUsersAsync(int currentUserId)
    {
        return await _db.Users
            .Where(u => u.Id != currentUserId)
            .OrderBy(u => u.Username)
            .Select(u => new UserDto { Id = u.Id, Username = u.Username })
            .ToListAsync();
    }
}