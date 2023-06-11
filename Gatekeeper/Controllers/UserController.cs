using Gatekeeper.Models;
using Gatekeeper.Services;
using Gatekeeper.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gatekeeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _config;
    private readonly MongoDbService _mongoDbService;
    
    public UserController(ILogger<UserController> logger, IConfiguration config, MongoDbService mongoDbService)
    {
        _logger = logger;
        _config = config;
        _mongoDbService = mongoDbService;
    }
    
    [Authorize]
    [HttpGet("readUserByUserId", Name = "ReadUserByUserId")]
    public IActionResult ReadUserByUserId(string userId)
    {
        var user = _mongoDbService.ReadByUserId(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok();
    }
    [Authorize]
    [HttpGet("readUserByUsername", Name = "ReadUserByUsername")]
    public IActionResult ReadUserByUsername(string username)
    {
        var user = _mongoDbService.ReadByUsername(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    [Authorize]
    [HttpGet("readUserByEmailAddress", Name = "ReadUserByEmailAddress")]
    public IActionResult ReadUserByEmailAddress(string emailAddress)
    {
        var user = _mongoDbService.ReadByEmailAddress(emailAddress);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    [HttpPost("createUser", Name = "CreateUser")]
    public IActionResult CreateUser(UserModel user)
    {
        if (_mongoDbService.ReadByUsername(user.username) != null)
        {
            return BadRequest("Username already exists");
        }
        if (_mongoDbService.ReadByEmailAddress(user.email) != null)
        {
            return BadRequest("Email address already exists");
        }

        user.password = SecurityUtil.HashPassword(user.password);
        _mongoDbService.CreateUser(user);
        return Ok();
    }
    [Authorize]
    [HttpPut("updateUser", Name = "UpdateUser")]
    public IActionResult UpdateUser(UserModel user)
    {
        if (_mongoDbService.ReadByUserId(user.userId) == null)
        {
            return NotFound();
        }
        if (_mongoDbService.ReadByUsername(user.username) != null)
        {
            return BadRequest("Username already exists");
        }
        if (_mongoDbService.ReadByEmailAddress(user.email) != null)
        {
            return BadRequest("Email address already exists");
        }

        user.password = SecurityUtil.HashPassword(user.password);
        _mongoDbService.UpdateUser(user);
        return Ok();
    }
    [Authorize]
    [HttpDelete("deleteUser", Name = "DeleteUser")]
    public IActionResult DeleteUser(string userId)
    {
        if (_mongoDbService.ReadByUserId(userId) == null)
        {
            return NotFound();
        }
        _mongoDbService.DeleteUser(userId);
        return Ok();
    }
    
    [HttpPost("loginByEmailAddress", Name = "LoginByEmailAddress")]
    public IActionResult LoginByEmailAddress([FromForm] string emailAddress, [FromForm] string password)
    {
        var user = _mongoDbService.ReadByEmailAddress(emailAddress);
        if (user == null)
        {
            return NotFound();
        }
        if (!SecurityUtil.VerifyPassword(password, user.password))
        {
            return BadRequest("Incorrect password");
        }
        var token = GenerateJSONWebTokenUtil.GenerateJSONWebToken(_config);
        return Ok(new
        {
            access_token = token,
        });
    }
    [HttpPost("loginByUsername", Name = "LoginByUsername")]
    public IActionResult LoginByUsername([FromForm] string username, [FromForm] string password)
    {
        var user = _mongoDbService.ReadByUsername(username);
        if (user == null)
        {
            return NotFound();
        }
        if (!SecurityUtil.VerifyPassword(password, user.password))
        {
            return BadRequest("Incorrect password");
        }
        var token = GenerateJSONWebTokenUtil.GenerateJSONWebToken(_config);
        return Ok(new
        {
            access_token = token,
        });
    }
}