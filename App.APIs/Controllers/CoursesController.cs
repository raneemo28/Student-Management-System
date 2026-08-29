using App.Application.CQRS.Courses.Commands;
using App.Application.CQRS.Courses.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _mediator.Send(new GetAllCoursesQuery());
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var course = await _mediator.Send(new GetCourseByIdQuery(id));
        if (course == null) return NotFound();
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
    {
        var result = await _mediator.Send(new CreateCourseCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Course_id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCourseDto dto)
    {
        var result = await _mediator.Send(new UpdateCourseCommand(id, dto));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
