using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterUserDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginUserDto dto);
}
