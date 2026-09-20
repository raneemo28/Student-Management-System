using App.Application.CQRS.Homeworks.Commands;
using App.Application.CQRS.Homeworks.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeworksController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeworksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("course/{courseId}")]
    [ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "courseId" })]
    public async Task<IActionResult> GetByCourse(string courseId)
    {
        var homeworks = await _mediator.Send(new GetHomeworksByCourseQuery(courseId));
        return Ok(homeworks);
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetById(string id)
    {
        var homework = await _mediator.Send(new GetHomeworkByIdQuery(id));
        if (homework == null) return NotFound();
        return Ok(homework);
    }

    [HttpPost("course/{courseId}")]
    public async Task<IActionResult> Create(string courseId, [FromForm] CreateHomeworkDto dto)
    {
        if (dto.QuestionFileBytes != null && dto.QuestionFileBytes.Length > 0)
        {
            dto.QuestionFileName = dto.QuestionFileName ?? "homework.pdf";
        }

        var result = await _mediator.Send(new CreateHomeworkCommand(courseId, dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Homework_id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromForm] UpdateHomeworkDto dto)
    {
        if (dto.QuestionFileBytes != null && dto.QuestionFileBytes.Length > 0)
        {
            dto.QuestionFileName = dto.QuestionFileName ?? "homework.pdf";
        }

        var result = await _mediator.Send(new UpdateHomeworkCommand(id, dto));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteHomeworkCommand(id));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
