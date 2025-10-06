using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StepCounter.Data;
using StepCounter.DTOs.Step;
using StepCounter.Services.StepIngestionService;

namespace StepCounter.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StepController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IStepIngestionService _stepIngestionService;
    
    public StepController(AppDbContext dbContext, IStepIngestionService stepIngestionService)
    {
        _dbContext = dbContext;
        _stepIngestionService = stepIngestionService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] StepBatchDto stepBatchDto)
    {
        await _stepIngestionService.ProcessBatchAsync(stepBatchDto);
        return Ok();
    }
}