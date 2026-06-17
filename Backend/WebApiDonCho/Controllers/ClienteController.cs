using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiDonCho.Services;

namespace WebApiDonCho.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClienteController(IUnitOfWork uow, ClienteService clienteService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resultado = await uow.FacClienteR.GetAllAsync();
        return Ok(resultado.OrderBy(x => x.Id));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await uow.FacClienteR.GetByIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpGet("cedula/{cedulaRuc}")]
    public async Task<IActionResult> GetByCedula(string cedulaRuc)
    {
        var cliente = await uow.FacClienteR.GetByCedulaRucAsync(cedulaRuc);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost("crear")]
    public async Task<IActionResult> Create([FromBody] FacClienteDTO cliente)
    {
        try
        {
            var cli = await clienteService.AddCliente(cliente);
            return CreatedAtAction(nameof(GetById), new { id = cli.Id }, cli);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FacCliente cliente)
    {
        if (id != cliente.Id) return BadRequest();
        uow.FacClienteR.Update(cliente);
        await uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await uow.FacClienteR.GetByIdAsync(id);
        if (cliente is null) return NotFound();
        uow.FacClienteR.Delete(cliente);
        await uow.SaveChangesAsync();
        return NoContent();
    }
}
