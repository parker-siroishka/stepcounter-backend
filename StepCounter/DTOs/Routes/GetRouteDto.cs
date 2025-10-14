namespace StepCounter.DTOs.Routes;

public class GetRouteDto
{
    public string Title { get; set; } = string.Empty;
    public Guid? CreatorId { get; set; }
    public string Description { get; set; } = string.Empty;
    public double TotalDistance { get; set; }
    public UserRouteProgressDto? RouteProgress { get; set; }
    public List<CoordinateDto> Coordinates { get; set; } = [];
    public List<CheckpointDto> Checkpoints { get; set; } = [];
}