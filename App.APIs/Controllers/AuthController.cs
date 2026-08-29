using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }
}
