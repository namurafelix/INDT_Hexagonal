using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Repositories;

public interface IContratacaoRepository
{
    Task AdicionarAsync(Contratacao contratacao);
    Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId);
}
