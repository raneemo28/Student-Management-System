using App.Application.DTOs;
using App.Application.Common;
using MediatR;

namespace App.Application.CQRS.Students.Commands;

public record CreateStudentCommand(CreateStudentDto Dto) : IRequest<Result<StudentDto>>;
