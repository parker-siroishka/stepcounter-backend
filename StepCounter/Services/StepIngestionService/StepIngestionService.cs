using Microsoft.EntityFrameworkCore;
using StepCounter.Data;
using StepCounter.DTOs.Step;
using StepCounter.Entities.Step;
using StepCounter.Services.RouteProgressService;

namespace StepCounter.Services.StepIngestionService;

public class StepIngestionService : IStepIngestionService
{
    private readonly AppDbContext _dbContext;
    private readonly IRouteProgressService _routeProgressService;

    public StepIngestionService(AppDbContext dbContext, IRouteProgressService routeRrogressService)
    {
        _dbContext = dbContext;
        _routeProgressService = routeRrogressService;
    }
    
    public async Task ProcessBatchAsync(StepBatchDto batchDto)
    {
        if (batchDto == null || batchDto.StepRecords == null || batchDto.StepRecords.Count == 0) return;
        
        var userId = batchDto.UserId;
        
        var startTimes = batchDto.StepRecords.Select(o => o.StartTime).ToList();
        var endTimes = batchDto.StepRecords.Select(o => o.EndTime).ToList();

        var existingRecords = await _dbContext.StepRecords
            .Where(r => r.UserId == userId &&
                        startTimes.Contains(r.StartTime) &&
                        endTimes.Contains(r.EndTime))
            .ToListAsync();

        var newRecords = batchDto.StepRecords
            .Where(r => !existingRecords.Any(e =>
                e.StartTime == r.StartTime &&
                e.EndTime == r.EndTime))
            .Select(r => new StepRecord
            {
                UserId = batchDto.UserId,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                Steps = r.Steps,
                DistanceTravelled = r.DistanceTravelled ?? 0,
                Source = r.Source ?? "Unknown",
                SourceId = r.SourceId ?? "Unknown",
                Processed = false,
                ReceivedAt = DateTimeOffset.UtcNow
            })
            .ToList();

        await _dbContext.StepRecords.AddRangeAsync(newRecords);
        await _dbContext.SaveChangesAsync();
        
        await _routeProgressService.UpdateUserRouteProgress(batchDto.UserId);
    }

}