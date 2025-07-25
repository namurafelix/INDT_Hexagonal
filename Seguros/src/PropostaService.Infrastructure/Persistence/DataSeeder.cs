using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(PropostaDbContext context)
    {
        if (!await context.Propostas.AnyAsync())
        {
            await context.Propostas.AddRangeAsync(
                new Proposta("Carlos Silva", "12345678900", 30, 15000m),
                new Proposta("Ana Souza", "98765432100", 40, 20000m)
            );
            await context.SaveChangesAsync();
        }
    }
}
