using Gatekeeper.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gatekeeper.Services;

public class MongoDbService
{
    private readonly IMongoCollection<UserModel> _users;

    public MongoDbService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _users = database.GetCollection<UserModel>("users");
    }
    
    //CRUD for UserModel
    public UserModel? ReadByUserId(string userId) =>
        _users.Find(user => user.userId == userId).FirstOrDefault();
    public UserModel? ReadByUsername(string username) =>
        _users.Find(user => user.username == username).FirstOrDefault();
    public UserModel? ReadByEmailAddress(string emailAddress) =>
        _users.Find(user => user.email == emailAddress).FirstOrDefault();
    public void CreateUser(UserModel user) => _users.InsertOne(user);
    public void UpdateUser(UserModel user) =>
        _users.ReplaceOne(usr => usr.userId == user.userId, user);
    public void DeleteUser(string userId) =>
        _users.DeleteOne(usr => usr.userId == userId);
}