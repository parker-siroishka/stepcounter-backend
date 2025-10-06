using StepCounter.DTOs.Step;

namespace StepCounter.Services.StepIngestionService;

public interface IStepIngestionService
{
    public Task ProcessBatchAsync(StepBatchDto stepBatchDto);
}