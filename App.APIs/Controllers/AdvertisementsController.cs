using App.Application.CQRS.Advertisements.Commands;
using App.Application.CQRS.Advertisements.Queries;
using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvertisementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdvertisementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var advertisements = await _mediator.Send(new GetAllAdvertisementsQuery());
        return Ok(advertisements);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var advertisement = await _mediator.Send(new GetAdvertisementByIdQuery(id));
        if (advertisement == null) return NotFound();
        return Ok(advertisement);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAdvertisementDto dto)
    {
        var result = await _mediator.Send(new CreateAdvertisementCommand(dto));
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Advertisement_id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAdvertisementDto dto)
    {
        var result = await _mediator.Send(new UpdateAdvertisementCommand(id, dto));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteAdvertisementCommand(id));
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });

        return NoContent();
    }
}
