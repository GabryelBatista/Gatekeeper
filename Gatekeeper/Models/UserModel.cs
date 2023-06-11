using System.Text.Json.Serialization;
using Gatekeeper.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Gatekeeper.Models;

public class UserModel
{
    [BsonId]
    public string userId { get; set; } = Guid.NewGuid().ToString();
    public string username { get; set; }
    [JsonIgnore]
    public string password { get; set; }
    public string email { get; set; }
    public Roles role { get; set; } = Roles.Unknown;
    
    public UserModel(string username, string password, string email)
    {
        this.username = username;
        this.password = password;
        this.email = email;
    }
}