using StepCounter.DTOs.Auth;
using StepCounter.Entities;
using StepCounter.Entities.User;

namespace StepCounter.Services;

public interface IAuthService
{
    public Task<AuthResponseDto?> LoginAsync(LoginUserDto loginUserDto, string password);
    public Task<RegisterUserDto> RegisterAsync(User userToRegister, string password);
    public (string token, DateTime expiresAt) GenerateAccessToken(User user);
    public string GenerateRefreshToken();
}