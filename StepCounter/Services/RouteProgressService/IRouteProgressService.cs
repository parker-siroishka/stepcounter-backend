namespace StepCounter.Services.RouteProgressService;

public interface IRouteProgressService
{
    public Task UpdateUserRouteProgress(Guid userId);
}