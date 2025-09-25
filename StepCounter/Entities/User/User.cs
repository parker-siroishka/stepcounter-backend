namespace StepCounter.Entities.User;

public class User : BaseEntity
{
    public Guid Id  { get; set; }
    public required string Username { get; set; }
    public string PasswordHash { get; set; } = null!;
    public required string Email { get; set; }
    public string AvatarUrl { get; set; } = null!;
    public required UserPreferences Preferences { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
}