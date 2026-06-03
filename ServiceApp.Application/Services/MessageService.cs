using ServiceApp.Application.DTOs;
using ServiceApp.Domain.Interfaces;

namespace ServiceApp.Application.Services;

public class MessageService
{
    private readonly IUserRepository _userRepository;
    private readonly IWhatsAppService _whatsAppService;

    public MessageService(IUserRepository userRepository, IWhatsAppService whatsAppService)
    {
        _userRepository = userRepository;
        _whatsAppService = whatsAppService;
    }

    public async Task<MessageResponseDto> SendMessageToUserWithLeastMessagesAsync()
    {
        var user = await _userRepository.GetUserWithLeastMessagesAsync();
        
        if (user == null)
        {
            return new MessageResponseDto
            {
                Success = false,
                Message = "No users found in database"
            };
        }

        var messageSent = await _whatsAppService.SendMessageAsync(user);


        return new MessageResponseDto
        {
            Success = !string.IsNullOrEmpty(messageSent),
            Message = messageSent,
            UserId = user._id,
            UserName = user.Name,
            PhoneNumber = user.PhoneNumber
        };
    }
}
