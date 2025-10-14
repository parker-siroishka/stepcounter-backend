using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StepCounter.Services.RouteService;

namespace StepCounter.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;
    
    
    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetRoutes()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);

        var routes = await _routeService.GetRoutesAsync(userId);
        return Ok(routes);
    }
}