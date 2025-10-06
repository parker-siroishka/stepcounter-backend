using StepCounter.DTOs.Auth;
using StepCounter.DTOs.User;
using StepCounter.Entities.Users;

namespace StepCounter.Services.AuthService;

public interface IAuthService
{
    public Task<AuthResponseDto?> LoginAsync(LoginUserDto loginUserDto, string password);
    public Task<RegisterUserDto> RegisterAsync(User userToRegister, string password);
    public (string token, DateTime expiresAt) GenerateAccessToken(User user);
    public string GenerateRefreshToken();
}