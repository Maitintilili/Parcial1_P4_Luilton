using Microsoft.AspNetCore.Mvc;
using Primer_Parcial_Luilton.Services;
using Primer_Parcial_Luilton.Models;

namespace Primer_Parcial_Luilton.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NumberController(NumbersService service, ILogger<NumberController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NumberRecordGet>> Post(NumberRecordSet request)
    {
        var guardado = await service.SaveAsync(request);
        logger.LogInformation("Guardado registro {Id}", guardado.Id);
        return CreatedAtAction(nameof(Get), new { id = guardado.Id }, guardado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, NumberRecordSet request)
    {
        var ok = await service.UpdateAsync(id, request);
        if (!ok) logger.LogWarning("Intento de actualizar Id {Id} que no existe", id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NumberRecordGet>> Get(int id) =>
        await service.GetByIdAsync(id) is { } r ? Ok(r) : NotFound();

    [HttpGet]
    public async Task<ActionResult<List<NumberRecordGet>>> List() =>
        Ok(await service.GetListAsync());
}

