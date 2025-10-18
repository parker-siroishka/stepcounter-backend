namespace StepCounter.DTOs.Routes;

public class UserRouteProgressDto
{
    public Guid RouteId { get; set; }
    public int Steps { get; set; }
    public double DistanceTravelled { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public float PercentComplete { get; set; }
    public CoordinateDto? CurrentLocation { get; set; }
}