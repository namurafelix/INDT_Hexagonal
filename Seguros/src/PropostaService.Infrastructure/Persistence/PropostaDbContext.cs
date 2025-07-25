using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Persistence;

public class PropostaDbContext : DbContext
{
    public DbSet<Proposta> Propostas => Set<Proposta>();

    public PropostaDbContext(DbContextOptions<PropostaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proposta>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.NomeCliente).IsRequired();
            entity.Property(p => p.CPF).IsRequired();
            entity.Property(p => p.Idade).IsRequired();
            entity.Property(p => p.ValorSeguro).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Status).IsRequired();
        });
    }
}
