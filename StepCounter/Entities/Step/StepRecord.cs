using StepCounter.Entities.Users;

namespace StepCounter.Entities.Step;

public class StepRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public int Steps { get; set; }
    public double DistanceTravelled { get; set; }
    public string Source { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public bool Processed { get; set; } = false;
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
}