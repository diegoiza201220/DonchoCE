using EFModel.DTO;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Models;
using Infoware.SRI.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiDonCho.Services;

namespace WebApiDonCho.Controllers;

// ── Celcertificado ────────────────────────────────────────────────────────────
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CelcertificadoController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public CelcertificadoController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _uow.CelCertificadoR.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _uow.CelCertificadoR.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CelCertificadoDTO item)
    {
        //TODO: await _uow.CelCertificadoR.AddAsync(item);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CelCertificadoDTO item)
    {
        if (id != item.Id) return BadRequest();
        //_uow.CelCertificadoR.Update(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _uow.CelCertificadoR.GetByIdAsync(id);
        if (item is null) return NotFound();
        _uow.CelCertificadoR.Delete(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

// ── CellogDocumento ───────────────────────────────────────────────────────────
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CellogDocumentoController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public CellogDocumentoController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _uow.CelLogDocumentoR.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _uow.CelLogDocumentoR.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CelLogDocumento item)
    {
        await _uow.CelLogDocumentoR.AddAsync(item);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CelLogDocumento item)
    {
        if (id != item.Id) return BadRequest();
        _uow.CelLogDocumentoR.Update(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _uow.CelLogDocumentoR.GetByIdAsync(id);
        if (item is null) return NotFound();
        _uow.CelLogDocumentoR.Delete(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

// ── CelsecuenciaSri ───────────────────────────────────────────────────────────
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CelsecuenciaSriController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public CelsecuenciaSriController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _uow.CelSecuenciasSriR.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _uow.CelSecuenciasSriR.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CelSecuenciaSri item)
    {
        await _uow.CelSecuenciasSriR.AddAsync(item);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CelSecuenciaSri item)
    {
        if (id != item.Id) return BadRequest();
        _uow.CelSecuenciasSriR.Update(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _uow.CelSecuenciasSriR.GetByIdAsync(id);
        if (item is null) return NotFound();
        _uow.CelSecuenciasSriR.Delete(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

// ── Genparametro ──────────────────────────────────────────────────────────────
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GenparametroController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public GenparametroController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _uow.GenParametroR.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _uow.GenParametroR.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GenParametro item)
    {
        await _uow.GenParametroR.AddAsync(item);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] GenParametro item)
    {
        if (id != item.Id) return BadRequest();
        _uow.GenParametroR.Update(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _uow.GenParametroR.GetByIdAsync(id);
        if (item is null) return NotFound();
        _uow.GenParametroR.Delete(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

// ── Secuencium ────────────────────────────────────────────────────────────────
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FacSecuenciaDiaController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public FacSecuenciaDiaController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var facsecuenciadia = await _uow.FacSecuenciaDiaR.GetAllAsync();
        //elapsedMs = watch.ElapsedMilliseconds;
        //Console.WriteLine($" tiempo CON ALL 02: {elapsedMs}");
        return Ok(new { facsecuenciadia });
        //Ok(await _uow.FacSecuenciaDiaR.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _uow.FacSecuenciaDiaR.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FacSecuenciaDia item)
    {
        await _uow.FacSecuenciaDiaR.AddAsync(item);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }
}

// ── Catalogos ────────────────────────────────────────────────────────────────
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GenCatalogoDetalleController(IUnitOfWork uow) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;

    [HttpPost("getallbynombrecatalogo")]
    public async Task<IActionResult> GetAllByNombreCatalogo([FromBody] RqConsultas item)
    {
        //Ok(await _ordenService.GetProductosVendidosPorFechaAsync(detalle));
        var gencatalogodetalle = _uow.GenCatalogoDetalleR.GetByCatalogoNombre(item.ValorString1);
        return Ok(gencatalogodetalle);
    }
}