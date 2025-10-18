using AutoMapper;
using StepCounter.DTOs.Routes;
using StepCounter.Entities;
using Route = StepCounter.Entities.Routes.Route;

namespace StepCounter.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Route, GetRouteDto>()
            .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src =>
                src.RouteGeometry.Coordinates.Select(c => new CoordinateDto
                {
                    Latitude = c.Y,
                    Longitude = c.X
                }).ToList()));
        
        CreateMap<UserRouteProgress, UserRouteProgressDto>()
            .ForMember(dest => dest.PercentComplete, opt => opt.MapFrom(src => 
                src.Route != null && src.Route.TotalDistance > 0 
                    ? (float)(src.DistanceTravelled / src.Route.TotalDistance) 
                    : 0f))
            .ForMember(dest => dest.CurrentLocation, opt => opt.Ignore()); // We'll calculate this in the service
    }
}