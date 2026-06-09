using ServiceApp.Domain.Entities;

namespace ServiceApp.Domain.Interfaces
{
    public interface IUserLoginRepository
    {
        Task<UserLogin?> GetByIdAsync(string identification, string password);
    }
}