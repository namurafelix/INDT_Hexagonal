using PropostaService.Domain.Entities;
using PropostaService.Domain.Repositories;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.Services;

public class PropostaAppService
{
    private readonly IPropostaRepository _propostaRepository;

    public PropostaAppService(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<PropostaViewModel> CriarPropostaAsync(PropostaInputModel inputModel)
    {
        var proposta = new Proposta(inputModel.NomeCliente, inputModel.CPF, inputModel.Idade, inputModel.ValorSeguro);
        await _propostaRepository.AdicionarAsync(proposta);
        return new PropostaViewModel(proposta.Id, proposta.NomeCliente, proposta.CPF, proposta.Idade, proposta.ValorSeguro, proposta.Status.ToString());
    }

    public async Task<PropostaViewModel?> ObterPropostaPorIdAsync(Guid id)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(id);
        return proposta == null ? null : new PropostaViewModel(proposta.Id, proposta.NomeCliente, proposta.CPF, proposta.Idade, proposta.ValorSeguro, proposta.Status.ToString());
    }

    public async Task<IEnumerable<PropostaViewModel>> ListarTodasPropostasAsync()
    {
        var propostas = await _propostaRepository.ListarTodasAsync();
        return propostas.Select(p => new PropostaViewModel(p.Id, p.NomeCliente, p.CPF, p.Idade, p.ValorSeguro, p.Status.ToString()));
    }

    public async Task AprovarPropostaAsync(Guid id)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(id);
        if (proposta == null)
        {
            throw new KeyNotFoundException($"Proposta com ID {id} não encontrada.");
        }
        proposta.Aprovar();
        await _propostaRepository.AtualizarAsync(proposta);
    }

    public async Task RecusarPropostaAsync(Guid id)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(id);
        if (proposta == null)
        {
            throw new KeyNotFoundException($"Proposta com ID {id} não encontrada.");
        }
        proposta.Recusar();
        await _propostaRepository.AtualizarAsync(proposta);
    }
}
