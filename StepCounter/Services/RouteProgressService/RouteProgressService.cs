using Microsoft.EntityFrameworkCore;
using StepCounter.Data;

namespace StepCounter.Services.RouteProgressService;

public class RouteProgressService : IRouteProgressService
{
    private readonly AppDbContext _dbContext;
    
    public RouteProgressService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateUserRouteProgress(Guid userId)
    {
        var activeRoutes = await _dbContext.UserRouteProgress
            .Where(p => p.UserId == userId && p.CompletedAt == null)
            .Include(p => p.Route)
            .ToListAsync();
        
        if (activeRoutes.Count() == 0) return;

        var newStepRecords = await _dbContext.StepRecords
            .Where(r => r.UserId == userId && !r.Processed)
            .ToListAsync();
        
        if (newStepRecords.Count() == 0) return;

        var cumulativeSteps = newStepRecords.Sum(r => r.Steps);
        var cumulativeDistance = newStepRecords.Sum(r => r.DistanceTravelled);
        
        foreach (var progress in activeRoutes)
        {
            progress.DistanceTravelled += cumulativeDistance;
            progress.Steps += cumulativeSteps;
            
            if (progress.DistanceTravelled >= progress.Route!.TotalDistance)
            {
                progress.CompletedAt = DateTimeOffset.UtcNow;
            }
        }
        
        foreach (var record in newStepRecords)
        {
            record.Processed = true;
        }
        
        await _dbContext.SaveChangesAsync();

    }
}