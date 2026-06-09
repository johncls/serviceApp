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
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Eq(u => u.Identification, user.Identification),
            Builders<User>.Filter.Eq(u => u.PhoneNumber, user.PhoneNumber)
        );
        
        var existingUser = await _context.Users.Find(filter).FirstOrDefaultAsync();

        if (existingUser != null)
        {
            if (existingUser.Identification == user.Identification)
            {
                throw new Exception("User with this identification already exists");
            }

            throw new Exception("User with this phone number already exists");
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

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetAllUsersListAsync(int page = 1, int pageSize = 10)
    {
        return _context.Users
            .Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public Task ResetAllCountersAsync()
    {
        return _context.Users.UpdateManyAsync(_ => true, Builders<User>.Update.Set(u => u.MessageCount, 0));
    }
}
