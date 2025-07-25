using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Repositories;

namespace ContratacaoService.Application.Services;

public class ContratacaoAppService
{
    private readonly IContratacaoRepository _repository;
    private readonly IPropostaServiceClient _propostaClient;

    public ContratacaoAppService(IContratacaoRepository repository, IPropostaServiceClient propostaClient)
    {
        _repository = repository;
        _propostaClient = propostaClient;
    }

    public async Task ContratarAsync(Guid propostaId)
    {
        var status = await _propostaClient.ObterStatusPropostaAsync(propostaId);

        if (status != "Aprovada")
            throw new InvalidOperationException("Proposta não aprovada.");

        var contratacao = new Contratacao { PropostaId = propostaId };
        await _repository.AdicionarAsync(contratacao);
    }
}
