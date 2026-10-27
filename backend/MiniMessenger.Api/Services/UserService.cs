// using Microsoft.EntityFrameworkCore;
// using MiniMessenger.Api.Data;
// using MiniMessenger.Api.DTOs;
// using MiniMessenger.Api.Services.Interfaces;

// namespace MiniMessenger.Api.Services;

// public class UserService : IUserService
// {
//     private readonly AppDbContext _db;

//     public UserService(AppDbContext db)
//     {
//         _db = db;
//     }

//     public async Task<List<UserDto>> GetOtherUsersAsync(int currentUserId)
//     {
//         return await _db.Users
//             .Where(u => u.Id != currentUserId)
//             .OrderBy(u => u.Username)
//             .Select(u => new UserDto { Id = u.Id, Username = u.Username })
//             .ToListAsync();
//     }
// }


// (On remplace AppDbContext par IMongoDatabase, on récupère la collection Users
// , et on adapte la requête LINQ compatible MongoDB)


using MongoDB.Driver;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Models;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Services;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService(IMongoDatabase database)
    {
        _usersCollection = database.GetCollection<User>("Users");
    }

    public async Task<List<UserDto>> GetOtherUsersAsync(string currentUserId)
    {
        // On récupère tous les utilisateurs sauf celui qui est connecté
        var users = await _usersCollection
            .Find(u => u.Id != currentUserId)
            .SortBy(u => u.Username)
            .ToListAsync();

        // On mappe vers le DTO
        return users.Select(u => new UserDto 
        { 
            Id = u.Id, 
            Username = u.Username 
        }).ToList();
    }
}