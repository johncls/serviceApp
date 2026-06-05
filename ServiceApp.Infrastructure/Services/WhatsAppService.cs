using Microsoft.Extensions.Logging;
using ServiceApp.Domain.Entities;
using ServiceApp.Domain.Interfaces;

namespace ServiceApp.Infrastructure.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly string _apiUrl;
    private readonly string _apiToken;
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppService> _logger;
    private readonly IUserRepository _userRepository;

    public WhatsAppService(string apiUrl, string apiToken, HttpClient httpClient, ILogger<WhatsAppService> logger, IUserRepository userRepository)
    {
        _apiUrl = apiUrl;
        _apiToken = apiToken;
        _httpClient = httpClient;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<string> SendMessageAsync(User user )
    {
        try
        {
            var phone = user.PhoneNumber;
            var message = Uri.EscapeDataString(user.Message);
            string url = "";

            if(string.IsNullOrEmpty(message)){
                url = $"https://chat.whatsapp.com/{phone}";
                 
            }else{
                url = $"https://chat.whatsapp.com/{phone}?text={message}";
            }

            if(string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            user.MessageCount++;
            user.LastMessageAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return url;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending WhatsApp message: {ex.Message}");
            return string.Empty;
        }
    }
}
