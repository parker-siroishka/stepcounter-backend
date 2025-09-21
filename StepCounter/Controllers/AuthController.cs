using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepCounter.Data;
using StepCounter.Entities;
using StepCounter.Entities.User;
using StepCounter.Services;

namespace StepCounter.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IAuthService _authService;

    public AuthController(AppDbContext dbContext, IAuthService authService)
    {
        _dbContext = dbContext;
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        var loggedInUser = await _authService.LoginAsync(loginUserDto, loginUserDto.Password);
        if (loggedInUser == null) return BadRequest(new { message = "Username or password is incorrect" });
        return Ok(loggedInUser);
        
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(o => o.Email == registerUserDto.Email || o.Username == registerUserDto.UserName);
        if (existingUser != null) return BadRequest("User already exists");

        var userToRegister = new User
        {
            Id = Guid.NewGuid(),
            Username = registerUserDto.UserName,
            Email = registerUserDto.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = registerUserDto.AvatarUrl,
            Preferences = new UserPreferences()
        };
        
        var registeredUser = await _authService.RegisterAsync(userToRegister, registerUserDto.Password);
        return Ok(new { registeredUser.UserName, registeredUser.Email });

    }
    
    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        return Ok(new { message = "You accessed a protected endpoint!" });
    }
}