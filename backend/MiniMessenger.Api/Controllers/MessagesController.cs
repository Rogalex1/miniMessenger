using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Extensions;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<MessageDto>>> GetConversation(string userId)
    {
        var messages = await _messageService.GetConversationAsync(User.GetUserId(), userId);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Send(SendMessageDto dto)
    {
        var message = await _messageService.SendAsync(User.GetUserId(), dto);

        if (message is null)
            return BadRequest(new { message = "Destinataire invalide." });

        return Ok(message);
    }
}