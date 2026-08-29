using App.Application.CQRS.HomeworkSubmissions.Commands;
using App.Application.CQRS.HomeworkSubmissions.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeworkSubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeworkSubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("homework/{homeworkId}")]
    public async Task<IActionResult> GetByHomework(string homeworkId)
    {
        var submissions = await _mediator.Send(new GetSubmissionsByHomeworkQuery(homeworkId));
        return Ok(submissions);
    }

    [HttpGet("student/{studentId}/homework/{homeworkId}")]
    public async Task<IActionResult> GetByStudent(string studentId, string homeworkId)
    {
        var submission = await _mediator.Send(new GetSubmissionByStudentQuery(studentId, homeworkId));
        if (submission == null) return NotFound();
        return Ok(submission);
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromForm] CreateHomeworkSubmissionDto dto)
    {
        if (dto.SolutionFileBytes != null && dto.SolutionFileBytes.Length > 0)
        {
            dto.SolutionFileName = dto.SolutionFileName ?? "solution.pdf";
        }

        var result = await _mediator.Send(new SubmitHomeworkCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetByStudent), new { studentId = result.Value!.Student_id, homeworkId = result.Value.Homework_id }, result.Value);
    }

    [HttpDelete("{submissionId}")]
    public async Task<IActionResult> Delete(string submissionId)
    {
        var result = await _mediator.Send(new DeleteHomeworkSubmissionCommand(submissionId));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
