using PropostaService.Domain.Entities;

namespace PropostaService.Domain.Repositories;

public interface IPropostaRepository
{
    Task AdicionarAsync(Proposta proposta);
    Task<Proposta?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Proposta>> ListarTodasAsync();
    Task AtualizarAsync(Proposta proposta);
}
