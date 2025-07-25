using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Repositories;
using PropostaService.Infrastructure.Persistence;

namespace PropostaService.Infrastructure.Repositories;

public class PropostaRepository : IPropostaRepository
{
    private readonly PropostaDbContext _context;

    public PropostaRepository(PropostaDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Proposta proposta)
    {
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();
    }

    public async Task<Proposta?> ObterPorIdAsync(Guid id)
    {
        return await _context.Propostas.FindAsync(id);
    }

    public async Task<IEnumerable<Proposta>> ListarTodasAsync()
    {
        return await _context.Propostas.ToListAsync();
    }

    public async Task AtualizarAsync(Proposta proposta)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync();
    }
}
