using MiniMessenger.Api.DTOs;

namespace MiniMessenger.Api.Services.Interfaces;

public interface IAuthService
{
    // Retourne null si le nom d'utilisateur existe déjà
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    // Retourne null si les identifiants sont invalides
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}