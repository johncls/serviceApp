namespace ServiceApp.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class User
{
    public string _id { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public bool Status { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public bool IsActive { get; set; } = true;

}

public class UserLogin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}