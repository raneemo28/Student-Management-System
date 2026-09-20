using App.Application.CQRS.Instructors.Commands;
using App.Application.CQRS.Instructors.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstructorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ResponseCache(Duration = 20, VaryByQueryKeys = new string[0])]
    public async Task<IActionResult> GetAll()
    {
        var instructors = await _mediator.Send(new GetAllInstructorsQuery());
        return Ok(instructors);
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetById(string id)
    {
        var instructor = await _mediator.Send(new GetInstructorByIdQuery(id));
        if (instructor == null) return NotFound();
        return Ok(instructor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInstructorDto dto)
    {
        var result = await _mediator.Send(new CreateInstructorCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Instructor_id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateInstructorDto dto)
    {
        var result = await _mediator.Send(new UpdateInstructorCommand(id, dto));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteInstructorCommand(id));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
