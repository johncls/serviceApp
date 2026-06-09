using System.Text;
using MongoDB.Driver;
using ServiceApp.Domain.Entities;
using ServiceApp.Domain.Interfaces;

namespace ServiceApp.Infrastructure.Persistence
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly MongoDbContext _context;

        public UserLoginRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<UserLogin?> GetByIdAsync(string identification, string password)
        {
            var passwordHash = Base64Encode(password);
            var filter = Builders<UserLogin>.Filter.And(
                Builders<UserLogin>.Filter.Eq(u => u.Identification, identification),
                Builders<UserLogin>.Filter.Eq(u => u.Password, passwordHash)
            );
            Console.WriteLine(filter);
            return await _context.UserLogins.Find(filter).FirstOrDefaultAsync();
        }

        private string Base64Encode(string value)
        {
            byte[] base64Bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(base64Bytes);
        }

    }
}