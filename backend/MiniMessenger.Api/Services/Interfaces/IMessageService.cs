// using MiniMessenger.Api.DTOs;

// namespace MiniMessenger.Api.Services.Interfaces;

// public interface IMessageService
// {
//     Task<List<MessageDto>> GetConversationAsync(int currentUserId, int otherUserId);

//     // Retourne null si le destinataire n'existe pas ou si c'est soi-même
//     Task<MessageDto?> SendAsync(int senderId, SendMessageDto dto);
// }


using MiniMessenger.Api.DTOs;

namespace MiniMessenger.Api.Services.Interfaces;

public interface IMessageService
{
    Task<List<MessageDto>> GetConversationAsync(string currentUserId, string otherUserId);

    // Retourne null si le destinataire n'existe pas ou si c'est soi-même
    Task<MessageDto?> SendAsync(string senderId, SendMessageDto dto);
}