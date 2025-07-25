using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.DTOs;
using PropostaService.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropostaService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropostasController : ControllerBase
{
    private readonly PropostaAppService _propostaAppService;

    public PropostasController(PropostaAppService propostaAppService)
    {
        _propostaAppService = propostaAppService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarProposta([FromBody] PropostaInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var propostaViewModel = await _propostaAppService.CriarPropostaAsync(inputModel);
        return CreatedAtAction(nameof(ObterPropostaPorId), new { id = propostaViewModel.Id }, propostaViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ListarPropostas()
    {
        var propostas = await _propostaAppService.ListarTodasPropostasAsync();
        return Ok(propostas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPropostaPorId(Guid id)
    {
        var proposta = await _propostaAppService.ObterPropostaPorIdAsync(id);
        if (proposta == null)
        {
            return NotFound();
        }
        return Ok(proposta);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlterarStatusProposta(Guid id, [FromBody] string status)
    {
        try
        {
            switch (status.ToLower())
            {
                case "aprovada":
                    await _propostaAppService.AprovarPropostaAsync(id);
                    break;
                case "recusada":
                    await _propostaAppService.RecusarPropostaAsync(id);
                    break;
                default:
                    return BadRequest("Status inválido. Use 'Aprovada' ou 'Recusada'.");
            }
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }
}
