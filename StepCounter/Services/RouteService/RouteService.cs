using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            .Include(r => r.RouteProgress)
            .AsNoTracking()
            .ToListAsync();
        
        var routeDtos = _mapper.Map<List<GetRouteDto>>(routes);
        
        foreach (var routeDto in routeDtos)
        {
            if (routeDto.RouteProgress != null && routeDto.RouteProgress.DistanceTravelled > 0)
            {
                var coordinateResult = await GetLocationAlongRouteAsync(
                    routeDto.RouteProgress.RouteId, 
                    routeDto.RouteProgress.DistanceTravelled);
                
                if (coordinateResult != null)
                {
                    routeDto.RouteProgress.CurrentLocation = new CoordinateDto
                    {
                        Latitude = coordinateResult.Latitude,
                        Longitude = coordinateResult.Longitude
                    };
                }
            }
        }
        
        return routeDtos;
    }
    
    private async Task<CoordinateResult?> GetLocationAlongRouteAsync(Guid routeId, double distanceTravelled)
    {
        var result = await _dbContext.Database.SqlQueryRaw<CoordinateResult>(
            @"SELECT 
                ST_X(ST_LineInterpolatePoint(""RouteGeometry"", @distance / ST_Length(""RouteGeometry""::geography))) AS ""Longitude"",
                ST_Y(ST_LineInterpolatePoint(""RouteGeometry"", @distance / ST_Length(""RouteGeometry""::geography))) AS ""Latitude""
              FROM ""Routes"" 
              WHERE ""Id"" = @routeId",
            new NpgsqlParameter("@distance", distanceTravelled),
            new NpgsqlParameter("@routeId", routeId))
            .FirstOrDefaultAsync();
        
        return result;
    }

}