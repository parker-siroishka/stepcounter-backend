namespace StepCounter.DTOs.Step;

public class StepRecordDto
{
    public string? UserId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public int Steps { get; set; }
    public double? DistanceTravelled { get; set; }
    public string? Source { get; set; } = "";
    public string? SourceId { get; set; } = "";
    public bool Processed { get; set; } = false;
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
}