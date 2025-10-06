namespace StepCounter.DTOs.Step;

public class StepBatchDto
{
    public Guid UserId { get; set; }
    public List<StepRecordDto>? StepRecords { get; set; }
}