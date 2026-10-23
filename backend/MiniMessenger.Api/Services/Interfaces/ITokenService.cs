using MiniMessenger.Api.Models;

namespace MiniMessenger.Api.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}