using ServiceApp.Domain.Entities;

namespace ServiceApp.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserWithLeastMessagesAsync();
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetByIdAsync(string identification);
    Task UpdateAsync(User user);
    Task<User> CreateAsync(User user);
    Task DeleteByIdAsync(string id);
    Task<List<User>> GetAllUsersListAsync(int page = 1, int pageSize = 10);
    Task ResetAllCountersAsync();
}
