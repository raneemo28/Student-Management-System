using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeightsController : ControllerBase
{
    private readonly ICourseService _courseService;

    public WeightsController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("course/{courseId}")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "courseId" })]
    public async Task<IActionResult> GetWeights(string courseId)
    {
        var courseResult = await _courseService.GetByIdAsync(courseId);
        if (!courseResult.IsSuccess)
            return NotFound(new { message = courseResult.Error });

        var weights = new CourseWeightsDto
        {
            WorkWeight = courseResult.Value!.WorkWeight,
            FirstExamWeight = courseResult.Value.FirstExamWeight,
            SecondExamWeight = courseResult.Value.SecondExamWeight,
            FinalExamWeight = courseResult.Value.FinalExamWeight
        };

        return Ok(weights);
    }

    [HttpPut("course/{courseId}")]
    public async Task<IActionResult> UpdateWeights(string courseId, [FromBody] CourseWeightsDto dto)
    {
        var result = await _courseService.UpdateWeightsAsync(courseId, dto);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result);
    }
}
