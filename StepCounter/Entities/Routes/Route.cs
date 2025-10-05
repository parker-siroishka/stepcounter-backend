using NetTopologySuite.Geometries;
using StepCounter.Entities.Users;

namespace StepCounter.Entities.Routes;

public class Route : BaseEntity
{
    public Guid Id { get; set; }
    public Guid? CreatorId { get; set; }
    public User? Creator { get; set; }
    public string Title { get; set; } = "My Route";
    public string Description { get; set; } = "";
    public LineString RouteGeometry { get; set; } = null!;
    public ICollection<Checkpoint> Checkpoints { get; set; } = new List<Checkpoint>();
    public ICollection<UserRouteProgress> RoutesProgress { get; set; } = new List<UserRouteProgress>();
}