using App.Application.CQRS.Enrollments.Commands;
using App.Application.CQRS.Enrollments.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ResponseCache(Duration = 15, VaryByQueryKeys = new string[0])]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await _mediator.Send(new GetAllEnrollmentsQuery());
        return Ok(enrollments);
    }

    [HttpGet("{studentId}/{courseId}")]
    [ResponseCache(Duration = 15, VaryByQueryKeys = new[] { "studentId", "courseId" })]
    public async Task<IActionResult> GetById(string studentId, string courseId)
    {
        var enrollment = await _mediator.Send(new GetEnrollmentByIdQuery(studentId, courseId));
        if (enrollment == null) return NotFound();
        return Ok(enrollment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseStudentDto dto)
    {
        var result = await _mediator.Send(new CreateEnrollmentCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { studentId = result.Value!.Student_id, courseId = result.Value.Course_id }, result.Value);
    }

    [HttpDelete("{studentId}/{courseId}")]
    public async Task<IActionResult> Delete(string studentId, string courseId)
    {
        var result = await _mediator.Send(new DeleteEnrollmentCommand(studentId, courseId));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
