using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StepCounter.Data;
using StepCounter.DTOs.Routes;

namespace StepCounter.Services.RouteService;

public class RouteService : IRouteService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public RouteService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<GetRouteDto>> GetRoutesAsync(Guid userId)
    {
        var routes = await _dbContext.Routes
            .Where(r => r.CreatorId == userId)
            .Include(r => r.Checkpoints)
            .Include(r => r.RouteProgress)
            .AsNoTracking()
            .ToListAsync();
        
        return _mapper.Map<List<GetRouteDto>>(routes);
    }

}