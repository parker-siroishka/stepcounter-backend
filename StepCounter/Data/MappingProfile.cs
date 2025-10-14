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
                }).ToList()))
            .ForMember(dest => dest.Checkpoints, opt => opt.MapFrom(src => 
                src.Checkpoints.Select(c => new CheckpointDto
                {
                    Order = c.Order,
                    Longitude = c.Location.X,
                    Latitude = c.Location.Y
                }).ToList()));
        
        CreateMap<UserRouteProgress, UserRouteProgressDto>();
    }
}