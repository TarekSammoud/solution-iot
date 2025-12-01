using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[ApiController]
[Route("api/unitemesure")]
public class UniteMesureController : ControllerBase
{
    private readonly IUniteMesureService _service;

    public UniteMesureController(IUniteMesureService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UniteMesureDto>>> GetAll([FromQuery] int? typeSonde)
    {
        if (typeSonde.HasValue)
        {
            if (!Enum.IsDefined(typeof(TypeSonde), typeSonde.Value))
            {
                return BadRequest("TypeSonde invalide.");
            }
            var filtered = await _service.GetByTypeSondeAsync((TypeSonde)typeSonde.Value);
            return Ok(filtered);
        }

        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UniteMesureDto>> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UniteMesureDto>> Create([FromBody] CreateUniteMesureDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UniteMesureDto>> Update(Guid id, [FromBody] UpdateUniteMesureDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans le body");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await _service.UpdateAsync(dto);
            if (updated is null)
            {
                return NotFound();
            }
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}
