using App.Application.CQRS.HomeworkQuestionMarks.Commands;
using App.Application.CQRS.HomeworkQuestionMarks.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeworkQuestionMarksController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeworkQuestionMarksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("submission/{submissionId}")]
    [ResponseCache(Duration = 5, VaryByQueryKeys = new[] { "submissionId" })]
    public async Task<IActionResult> GetBySubmission(string submissionId)
    {
        var questionMarks = await _mediator.Send(new GetQuestionMarksBySubmissionQuery(submissionId));
        return Ok(questionMarks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHomeworkQuestionMarkDto dto)
    {
        var result = await _mediator.Send(new CreateHomeworkQuestionMarkCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetBySubmission), new { submissionId = result.Value!.Submission_id }, result.Value);
    }

    [HttpDelete("{questionMarkId}")]
    public async Task<IActionResult> Delete(string questionMarkId)
    {
        var result = await _mediator.Send(new DeleteHomeworkQuestionMarkCommand(questionMarkId));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
