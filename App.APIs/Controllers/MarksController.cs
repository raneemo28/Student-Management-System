using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarksController : ControllerBase
{
    private readonly ICourseStudentService _courseStudentService;
    private readonly ICourseService _courseService;

    public MarksController(ICourseStudentService courseStudentService, ICourseService courseService)
    {
        _courseStudentService = courseStudentService;
        _courseService = courseService;
    }

    [HttpPut("enrollment/{studentId}/{courseId}")]
    public async Task<IActionResult> UpdateMarks(string studentId, string courseId, [FromBody] UpdateMarksDto dto)
    {
        var result = await _courseStudentService.UpdateMarksAsync(studentId, courseId, dto);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("enrollment/{studentId}/{courseId}")]
    public async Task<IActionResult> GetMarks(string studentId, string courseId)
    {
        var result = await _courseStudentService.GetMarksAsync(studentId, courseId);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("enrollment/{studentId}/{courseId}/final")]
    public async Task<IActionResult> GetFinalMark(string studentId, string courseId)
    {
        var marksResult = await _courseStudentService.GetMarksAsync(studentId, courseId);
        if (!marksResult.IsSuccess)
            return NotFound(new { message = marksResult.Error });

        var courseResult = await _courseService.GetByIdAsync(courseId);
        if (!courseResult.IsSuccess)
            return NotFound(new { message = courseResult.Error });

        var marks = marksResult.Value!;
        var weights = courseResult.Value!;

        double finalMark = 0;
        if (marks.WorkMark.HasValue) finalMark += marks.WorkMark.Value * weights.WorkWeight;
        if (marks.FirstExamMark.HasValue) finalMark += marks.FirstExamMark.Value * weights.FirstExamWeight;
        if (marks.SecondExamMark.HasValue) finalMark += marks.SecondExamMark.Value * weights.SecondExamWeight;

        return Ok(new { finalMark });
    }
}
