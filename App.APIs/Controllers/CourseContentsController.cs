using App.Application.CQRS.CourseContents.Commands;
using App.Application.CQRS.CourseContents.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseContentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseContentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(string courseId)
    {
        var contents = await _mediator.Send(new GetCourseContentsQuery(courseId));
        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var content = await _mediator.Send(new GetCourseContentByIdQuery(id));
        if (content == null) return NotFound();
        return Ok(content);
    }

    [HttpPost("course/{courseId}")]
    public async Task<IActionResult> Create(string courseId, [FromForm] CreateCourseContentDto dto, [FromForm] IFormFile file)
    {
        if (file != null)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            dto.FileBytes = memoryStream.ToArray();
            dto.FileName = file.FileName;
            dto.FileSize = file.Length;
        }

        var result = await _mediator.Send(new CreateCourseContentCommand(courseId, dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Content_id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromForm] UpdateCourseContentDto dto, [FromForm] IFormFile? file)
    {
        if (file != null)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            dto.FileBytes = memoryStream.ToArray();
            dto.FileSize = file.Length;
            dto.FileName = file.FileName;
        }

        var result = await _mediator.Send(new UpdateCourseContentCommand(id, dto));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteCourseContentCommand(id));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
