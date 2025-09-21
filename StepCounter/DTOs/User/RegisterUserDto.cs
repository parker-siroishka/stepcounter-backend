namespace StepCounter.Entities;

public class RegisterUserDto
{
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "Everyone";
    public string AvatarUrl { get; set; } = "";
}