using StepCounter.Entities.Users;
using Route = StepCounter.Entities.Routes.Route;

namespace StepCounter.Entities;

public class UserRouteProgress : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid RouteId { get; set; }
    public Route Route { get; set; } = null!;
    public double DistanceTravelled { get; set; }
    public int Steps { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}