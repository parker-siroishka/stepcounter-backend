using StepCounter.DTOs.Routes;

namespace StepCounter.Services.RouteService;

public interface IRouteService
{
    public Task<List<GetRouteDto>> GetRoutesAsync(Guid userId);
}