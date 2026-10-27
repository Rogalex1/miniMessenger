using MiniMessenger.Api.DTOs;

namespace MiniMessenger.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetOtherUsersAsync(string currentUserId);
}