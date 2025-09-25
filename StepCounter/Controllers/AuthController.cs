using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepCounter.Data;
using StepCounter.DTOs.Auth;
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
        var authResponseDto = await _authService.LoginAsync(loginUserDto, loginUserDto.Password);
        if (authResponseDto == null) return BadRequest(new { message = "Username or password is incorrect" });
        return Ok(authResponseDto);
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(o => o.Email == registerUserDto.Email || 
                                      o.Username == registerUserDto.UserName);
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

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(o => o.Id == refreshTokenDto.UserId);
        if (existingUser == null) return Unauthorized("User not found");
        if (existingUser.RefreshToken != refreshTokenDto.RefreshToken) return Unauthorized("Invalid refresh token");
        if (existingUser.RefreshTokenExpiryTime < DateTime.UtcNow) return Unauthorized("Refresh token expired");
        
        var (accessToken, accessTokenExpiresAt) = _authService.GenerateAccessToken(existingUser);
        var refreshToken = _authService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        existingUser.RefreshToken = refreshToken;
        existingUser.RefreshTokenExpiryTime = refreshTokenExpiresAt;
        await _dbContext.SaveChangesAsync();
        
        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Username = existingUser.Username
        });
    }
}