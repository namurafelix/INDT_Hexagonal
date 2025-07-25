using Microsoft.EntityFrameworkCore;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Repositories;
using ContratacaoService.Infrastructure.Persistence;

namespace ContratacaoService.Infrastructure.Repositories;

public class ContratacaoRepository : IContratacaoRepository
{
    private readonly ContratacaoDbContext _context;

    public ContratacaoRepository(ContratacaoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Contratacao contratacao)
    {
        await _context.Contratacoes.AddAsync(contratacao);
        await _context.SaveChangesAsync();
    }

    public async Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId)
    {
        return await _context.Contratacoes.FirstOrDefaultAsync(c => c.PropostaId == propostaId);
    }
}
