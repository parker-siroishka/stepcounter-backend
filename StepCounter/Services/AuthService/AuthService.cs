using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StepCounter.Data;
using StepCounter.DTOs.Auth;
using StepCounter.Entities;
using StepCounter.Entities.User;

namespace StepCounter.Services;

public class AuthService: IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    
    public AuthService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordHasher = new PasswordHasher<User>();
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginUserDto loginUserDto, string password)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(o => o.Username == loginUserDto.UserName || o.Email == loginUserDto.UserName);
        if (existingUser == null) return null;
        
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, loginUserDto.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        var (accessToken, accessTokenExpiresAt) = GenerateAccessToken(existingUser);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        
        existingUser.UpdatedAt = DateTime.UtcNow;
        existingUser.RefreshToken = refreshToken;
        existingUser.RefreshTokenExpiryTime = refreshTokenExpiresAt;
        await _dbContext.SaveChangesAsync();
        
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Username = existingUser.Username,
            AccessTokenExpiresAt = accessTokenExpiresAt,
        };
    }
    
    public async Task<RegisterUserDto> RegisterAsync(User user, string password)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        
        return new RegisterUserDto
        {
            UserName = user.Username,
            Email = user.Email,
        };
    }
    
    public (string token, DateTime expiresAt) GenerateAccessToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            }),
            IssuedAt = DateTime.UtcNow,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        };

        var token = handler.CreateToken(descriptor);
        return (handler.WriteToken(token), descriptor.Expires!.Value);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}