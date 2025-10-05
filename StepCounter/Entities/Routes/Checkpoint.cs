using NetTopologySuite.Geometries;

namespace StepCounter.Entities.Routes;

public class Checkpoint : BaseEntity
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public Route Route { get; set; } = null!;
    public Point Location { get; set; } = null!;
    public int Order { get; set; }
}