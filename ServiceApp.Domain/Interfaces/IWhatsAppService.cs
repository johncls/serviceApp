using ServiceApp.Domain.Entities;

namespace ServiceApp.Domain.Interfaces;

public interface IWhatsAppService
{
    Task<string> SendMessageAsync(User user);
}
