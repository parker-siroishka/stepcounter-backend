namespace StepCounter.Entities.User;

public class User
{
    public Guid Id  { get; set; }
    public required string Username { get; set; }
    public string PasswordHash { get; set; }
    public required string Email { get; set; }
    public string AvatarUrl { get; set; } = null!;
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required UserPreferences Preferences { get; set; }
    
}