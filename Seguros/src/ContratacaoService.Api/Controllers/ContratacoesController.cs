using Microsoft.AspNetCore.Mvc;
using ContratacaoService.Application.Services;
using System;
using System.Threading.Tasks;

namespace ContratacaoService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratacoesController : ControllerBase
{
    private readonly ContratacaoAppService _contratacaoAppService;

    public ContratacoesController(ContratacaoAppService contratacaoAppService)
    {
        _contratacaoAppService = contratacaoAppService;
    }

    [HttpPost]
    public async Task<IActionResult> ContratarProposta([FromBody] Guid propostaId)
    {
        try
        {
            await _contratacaoAppService.ContratarAsync(propostaId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
