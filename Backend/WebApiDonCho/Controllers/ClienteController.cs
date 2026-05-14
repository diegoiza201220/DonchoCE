using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFModel.Interfaces;
using EFModel.Models;

namespace WebApiDonCho.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ClienteController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _uow.FacClienteR.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _uow.FacClienteR.GetByIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpGet("cedula/{cedulaRuc}")]
    public async Task<IActionResult> GetByCedula(string cedulaRuc)
    {
        var cliente = await _uow.FacClienteR.GetByCedulaRucAsync(cedulaRuc);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FacCliente cliente)
    {
        await _uow.FacClienteR.AddAsync(cliente);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FacCliente cliente)
    {
        if (id != cliente.Id) return BadRequest();
        _uow.FacClienteR.Update(cliente);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _uow.FacClienteR.GetByIdAsync(id);
        if (cliente is null) return NotFound();
        _uow.FacClienteR.Delete(cliente);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
