using MongoDB.Driver;
using ServiceApp.Domain.Entities;

namespace ServiceApp.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<UserLogin> _userLoginCollection;
    private readonly IMongoCollection<User> _userCollection;
    public MongoDbContext(string connectionString, string databaseName, string collectionUserLogin, string collectionUser)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        _userLoginCollection = _database.GetCollection<UserLogin>(collectionUserLogin);
        _userCollection = _database.GetCollection<User>(collectionUser);
    }

    public IMongoCollection<User> Users => _userCollection;
    public IMongoCollection<UserLogin> UserLogins => _userLoginCollection;
    
}
