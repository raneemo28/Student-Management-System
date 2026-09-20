using App.Application.CQRS.HomeworkSolutions.Commands;
using App.Application.CQRS.HomeworkSolutions.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeworkSolutionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeworkSolutionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("homework/{homeworkId}")]
    [ResponseCache(Duration = 15, VaryByQueryKeys = new[] { "homeworkId" })]
    public async Task<IActionResult> GetByHomework(string homeworkId)
    {
        var solutions = await _mediator.Send(new GetSolutionsByHomeworkQuery(homeworkId));
        return Ok(solutions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateHomeworkSolutionDto dto)
    {
        if (dto.SolutionFileBytes != null && dto.SolutionFileBytes.Length > 0)
        {
            dto.SolutionFileName = dto.SolutionFileName ?? "solution.pdf";
        }

        var result = await _mediator.Send(new CreateHomeworkSolutionCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetByHomework), new { homeworkId = result.Value!.Homework_id }, result.Value);
    }

    [HttpDelete("{solutionId}")]
    public async Task<IActionResult> Delete(string solutionId)
    {
        var result = await _mediator.Send(new DeleteHomeworkSolutionCommand(solutionId));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
