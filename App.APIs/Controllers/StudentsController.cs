using App.Application.CQRS.Students.Commands;
using App.Application.CQRS.Students.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _mediator.Send(new GetAllStudentsQuery());
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var student = await _mediator.Send(new GetStudentByIdQuery(id));
        if (student == null) return NotFound();
        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
    {
        var result = await _mediator.Send(new CreateStudentCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Student_id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateStudentDto dto)
    {
        var result = await _mediator.Send(new UpdateStudentCommand(id, dto));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteStudentCommand(id));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
