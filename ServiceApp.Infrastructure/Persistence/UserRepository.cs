using MongoDB.Driver;
using ServiceApp.Domain.Entities;
using ServiceApp.Domain.Interfaces;

namespace ServiceApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserWithLeastMessagesAsync()
    {
        return await _context.Users
            .Find(x => x.IsActive == true)
            .SortBy(u => u.MessageCount)
            .ThenBy(u => u.LastMessageAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string identification)
    {
        return await _context.Users
            .Find(u => u.Identification == identification)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _context.Users.ReplaceOneAsync(u => u._id == user._id, user);
    }

    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await _context.Users
            .Find(u => u.Identification == user.Identification)
            .FirstOrDefaultAsync();

        if (existingUser != null)
        {
            throw new Exception("User already exists");
        }

        await _context.Users.InsertOneAsync(user);
        
        return user;
    }

    public Task DeleteByIdAsync(string id)
    {
        return _context.Users.DeleteOneAsync(u => u._id == id);
    }

    public Task<List<User>> GetAllUsersListAsync()
    {
        return _context.Users.
        Find(_ => true).ToListAsync();
    }
}
