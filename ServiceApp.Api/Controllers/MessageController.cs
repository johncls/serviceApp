using Microsoft.AspNetCore.Mvc;
using ServiceApp.Application.DTOs;
using ServiceApp.Application.Services;

namespace ServiceApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;

    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("send")]
    public async Task<ActionResult<UserResponseDto>> SendMessage()
    {
        var result = await _messageService.SendMessageToUserWithLeastMessagesAsync();
        
        if (!result.Success)
        {
            return StatusCode(500, result);
        }

        return Ok(result);
    }
}
