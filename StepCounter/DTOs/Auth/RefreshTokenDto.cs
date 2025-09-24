namespace StepCounter.DTOs.Auth;

public class RefreshTokenDto
{
    public string? RefreshToken { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
}