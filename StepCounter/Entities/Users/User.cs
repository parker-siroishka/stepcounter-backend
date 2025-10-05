using StepCounter.Entities.Step;
using Route = StepCounter.Entities.Routes.Route;

namespace StepCounter.Entities.Users;

public class User : BaseEntity
{
    public Guid Id  { get; set; }
    public required string Username { get; set; }
    public string PasswordHash { get; set; } = null!;
    public required string Email { get; set; }
    public string AvatarUrl { get; set; } = null!;
    public ICollection<Route> Routes { get; set; } = new List<Route>();
    public ICollection<UserRouteProgress> RoutesProgress { get; set; } = new List<UserRouteProgress>();
    public ICollection<StepRecord> StepRecords { get; set; } = new List<StepRecord>();
    public required string Role { get; set; }
    public required UserPreferences Preferences { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
    
}