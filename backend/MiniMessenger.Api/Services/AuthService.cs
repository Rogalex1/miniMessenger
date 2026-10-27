// using Microsoft.EntityFrameworkCore;
// using MiniMessenger.Api.Data;
// using MiniMessenger.Api.DTOs;
// using MiniMessenger.Api.Models;
// using MiniMessenger.Api.Services.Interfaces;

// namespace MiniMessenger.Api.Services;

// public class AuthService : IAuthService
// {
//     private readonly AppDbContext _db;
//     private readonly ITokenService _tokenService;

//     public AuthService(AppDbContext db, ITokenService tokenService)
//     {
//         _db = db;
//         _tokenService = tokenService;
//     }

//     public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
//     {
//         var exists = await _db.Users.AnyAsync(u => u.Username == dto.Username);
//         if (exists) return null;

//         var user = new User
//         {
//             Username = dto.Username,
//             PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
//         };

//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();

//         return ToResponse(user);
//     }

//     public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
//     {
//         var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

//         if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
//             return null;

//         return ToResponse(user);
//     }

//     private AuthResponseDto ToResponse(User user)
//     {
//         return new AuthResponseDto
//         {
//             UserId = user.Id,
//             Username = user.Username,
//             Token = _tokenService.CreateToken(user)
//         };
//     }
// }



using MongoDB.Driver;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Models;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Services;

public class AuthService : IAuthService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly ITokenService _tokenService;

    public AuthService(IMongoDatabase database, ITokenService tokenService)
    {
        _usersCollection = database.GetCollection<User>("Users");
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Vérifie si l'utilisateur existe déjà
        var existingUser = await _usersCollection
            .Find(u => u.Username == dto.Username)
            .FirstOrDefaultAsync();

        if (existingUser != null) return null;

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        // Insertion dans MongoDB (génère l'ObjectId automatiquement)
        await _usersCollection.InsertOneAsync(user);

        return ToResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _usersCollection
            .Find(u => u.Username == dto.Username)
            .FirstOrDefaultAsync();

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return ToResponse(user);
    }

    private AuthResponseDto ToResponse(User user)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Token = _tokenService.CreateToken(user)
        };
    }
}