using Microsoft.EntityFrameworkCore;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Infrastructure.Persistence;

public class ContratacaoDbContext : DbContext
{
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    public ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contratacao>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.PropostaId).IsRequired();
            entity.Property(c => c.DataContratacao).IsRequired();
        });
    }
}
